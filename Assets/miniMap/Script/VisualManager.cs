using System.Collections.Generic;
using UnityEngine;
using ViewProvider = UnityEngine.Vector4;

public class VisualManager : MonoBehaviour {

    public Camera occlusionCamera;

    public Material fogMaterial;
    public Material minMapMaterial;

    public ComputeShader fogCalculator;

    public Shader gaussian;

    public GameObject fog;

    public Material visualOcclusionMaterial;

    [Range(0.01f, 10)]
    public float refreshInterval = 0.5f;

    public int mapResolution = 256;



    public bool openGaussian = false;


    private RenderTexture miniMapTexture;
    private RenderTexture fogMap;

    private float remainTime;

    private int MainHandle = -1, ProduceFinalMapHandle = -1;

    // Use this for initialization
    void Start () {
        remainTime = 0.2f;
        MainHandle = fogCalculator.FindKernel("Main");
        ProduceFinalMapHandle = fogCalculator.FindKernel("ProduceFinalMap");
        if (MainHandle == -1)
        {
            Debug.Log("Computer Shader <fogCalculator> not found.");
        }

        miniMapTexture = new RenderTexture(mapResolution, mapResolution, 24)
        {
            enableRandomWrite = true
        };
        miniMapTexture.Create();

        occlusionCamera.targetTexture = miniMapTexture;
        occlusionCamera.cullingMask = (1 << 19);
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
        Camera.main.cullingMask &= (~(1 << 19));

        fogMap = new RenderTexture(mapResolution, mapResolution, 24)
        {
            enableRandomWrite = true
        };
        fogMap.Create();

        fogMaterial.SetTexture("_FogMap",fogMap);
        minMapMaterial.SetTexture("_FogMap", fogMap);

        gaussianMaterial = new Material(gaussian);

        fog.transform.parent = Camera.main.transform;
        fog.transform.localPosition = Vector3.forward;
        fog.GetComponent<MeshRenderer>().enabled = true;
    }

    void FixedUpdate()
    {
        if (!active_) return;
        if (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            return;
        }
        remainTime = refreshInterval;
        
        ViewProvider[] viewProviders;
        {
            List<ViewProvider> vPList = new List<ViewProvider>();
            Matrix4x4 mvp_mt = occlusionCamera.projectionMatrix * occlusionCamera.worldToCameraMatrix;
            foreach (var keyValuePair in visualProviders)
            {
                VisualProvider vp = keyValuePair.Value;
                if (!vp.active) continue;
                ViewProvider pos = vp.transform.position;
                pos = mvp_mt * pos;
                pos.w = vp.visualRange;
                if (pos.w != 0)
                {
                    if (pos.x > 1.2 || pos.y > 1.2 || pos.x < -1.2 || pos.y < -1.2)
                        continue;
                    pos.z = vp.noOcclusion ? 0 : 1;
                    vPList.Add(pos);
                }
            }
            viewProviders = vPList.ToArray();
        }//generate data
        if (viewProviders.Length != 0)
        {
            ComputeBuffer cb = new ComputeBuffer(viewProviders.Length,16);
            cb.SetData(viewProviders);
            fogCalculator.SetBuffer(MainHandle, "viewProviders", cb);
            fogCalculator.SetTexture(MainHandle, "_OcclusionMap", miniMapTexture);
            fogCalculator.SetTexture(MainHandle, "_FogMap", fogMap);
            fogCalculator.SetFloat("scale", 0.5f / occlusionCamera.orthographicSize);
            fogCalculator.SetFloat("resolution", mapResolution);
            ViewProvider[] output = new ViewProvider[5];
            int size = Mathf.Clamp(viewProviders.Length / 32,1,1000);
            
            fogCalculator.Dispatch(MainHandle, size, 1, 1);
            cb.GetData(output);
            cb.Dispose();
            if (openGaussian) Filter(fogMap);
        }

        fogCalculator.SetTexture(ProduceFinalMapHandle, "_FogMap", fogMap);
        fogCalculator.Dispatch(ProduceFinalMapHandle, mapResolution / 8, mapResolution / 8, 1);

        Matrix4x4 m = occlusionCamera.projectionMatrix * occlusionCamera.worldToCameraMatrix;
        fogMaterial.SetMatrix("_OcclusionCamera_MVP", m);
    }



    //manager of visual provider
    private Dictionary<ushort, VisualProvider> visualProviders = new Dictionary<ushort, VisualProvider>();
    private bool[] IDs = new bool[65535];
    public void Register(VisualProvider vp)
    {
        if (vp.id != 0)
            return;
        ushort id = (ushort)Random.Range(1, 65536);
        while (IDs[id] == true) id += 1;
        IDs[id] = true;
        vp.id = id;

        visualProviders.Add(id, vp);
    }

    public void Logout(VisualProvider vp)
    {
        if (vp.id == 0)
        {
            Debug.Log("You are trying to logout a visual provider which has not been registered yet.");
            return;
        }
        IDs[vp.id] = false;
        vp.id = 0;
        visualProviders.Remove(vp.id);
    }

    public void TryToLogout(VisualProvider vp)
    {
        if (vp.id == 0)
            return;
        IDs[vp.id] = false;
        vp.id = 0;
        visualProviders.Remove(vp.id);
    }

    private bool active_ = true;
    public void SetActive(bool active)
    {
        active_ = active;
        fog.SetActive(active);
    }
    
  


    // gaussian filter
    [Range(1, 8)]
    [SerializeField]
    public int m_downSample = 2;
    [Range(1, 4)]
    [SerializeField]
    public int m_iterations = 3;
    [Range(0.2f, 3f)]
    [SerializeField]
    public float m_blurSpread = 0.6f;

    private Material gaussianMaterial;

    void Filter(RenderTexture texture)
    {
        int w = (int)(texture.width / m_downSample);
        int h = (int)(texture.height / m_downSample);
        RenderTexture buffer0 = RenderTexture.GetTemporary(w, h);
        RenderTexture buffer1 = RenderTexture.GetTemporary(w, h);
        buffer0.filterMode = FilterMode.Bilinear;
        buffer1.filterMode = FilterMode.Bilinear;
        Graphics.Blit(texture, buffer0);

        for (int i = 0; i < m_iterations; i++)
        {
            gaussianMaterial.SetFloat("_BlurSpread", 1 + i * m_blurSpread);

            Graphics.Blit(buffer0, buffer1, gaussianMaterial, 0);
            Graphics.Blit(buffer1, buffer0, gaussianMaterial, 1);
        }
        Graphics.Blit(buffer0, texture);
        RenderTexture.ReleaseTemporary(buffer0);
        RenderTexture.ReleaseTemporary(buffer1);
    }
}