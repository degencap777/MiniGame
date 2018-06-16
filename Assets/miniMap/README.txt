#!# 请一定注意Layer 19会被占用以进行视野检测 #!#
#!# 请一定注意只会为主相机(tag为MainCamera,下同)附加迷雾 #!#
#!# 明亮区域跳动，则请尝试降低负荷 #!#



1.Manual
	(1) 视野遮挡物体：
		-通过为物体挂载VisualOcclusion来使其成为视野遮挡物。
		-对于视野遮挡物，MeshFilter和MeshRender是必须的，该物体的网格将用于检测视野遮挡，并且不会显示在主相机中。
		-注意，这将导致物体无法正确显示在主相机中！所以请使用空物体挂载遮挡模型网格来创造遮挡物。
		-该物体必须具有MeshCollider以进行射线检测。
		-当你发现某个遮挡物表现不正常时，尝试调整mesh大小或更改VisualManager设置。另外注意Y轴位置。
		-尽量使用更少的Mesh来完成场景视野遮挡。紧贴的物体可以使用同一个Mesh。
		-使用低面数的Box Mesh可以有效提高射线检测效率。

	(2) 为单位赋予视野：
		-您需要为其添加VisualProvider组件。
		-您可以更改视野范围，并设置是否显示辅助线。
		-请注意，直接修改Active属性而不是通过接口方法可能导致性能问题。

	(3) 检测单位是否在视野内：
		-您需要为待检测物体添加VisualTest组件。
		-请注意Y轴高度差可能越过视野遮挡物。
		-您可以勾选autoTest属性来使其isInVisual属性保持自动刷新，请注意该设置带来的性能问题，并只在必要时开启。
		-您可以勾选autoHideWhenOutOfVisual属性使其在视野外时自动隐藏(仅仅隐藏，并未被挂起)，同样，请注意性能问题。
		-关闭autoHideWhenOutOfVisual属性将使单位保持原有状态，不论它之前是隐藏还是显示。
		-您可以改变TestRadius来改变检测半径，请保证该值始终大于该单位可能暴露的最大视野半径。
		-您可以开启/关闭两个show选项来控制相应辅助线条的显示。

	(4) VisualManager：
		-请将VisualManager预置体放置于场景中，并调整miniMapCamera的位置与范围，该范围将决定迷雾的计算范围。同样贴图分辨率情况下，越大的范围意味着越低的效率与精度。
		-迷雾将在运行时添加，您可以通过相关方法控制开启/关闭。
		-您可以修改RefreshInterval控制迷雾计算频率。
		-您可以通过修改mapResolution至合适值以在性能与质量间获取平衡，请尽量选择64的倍数，该参数一旦确定无法在运行时更改。
		-您可以开启/关闭迷雾的高斯模糊，事实上完全关闭高斯模糊意味着进入像素风格。
		-您可以通过改变下方参数来改变改变迷雾的性状以及调整性能。

	(5) 迷雾：
		-您可以修改VisualManager预置体下的fog子物体的材质，或直接更改Material文件夹下的FogMaterial材质来改变迷雾的颜色以及已探索区域的亮度。
		-Fog在稍低于UI层显示，所以请确保您的相机开启了UI层。

	(6) 小地图：
		-通过使用MiniMap预置体以在界面上创建小地图。
		-通过为MiniMap添加遮罩贴图可以更改地图形状以及背景，mask贴图的alpha通道决定某一像素是否会被剪裁(无论该像素来自mask贴图rgb通道还是动态贴图)，mask贴图的rgb通道会在主贴图后方显示。
		-小地图默认显示Layer 20层的物体，您可以通过为单位附加处于该层的子物体来控制显示样式。
		-您可以更改miniMapCamera的AdjustMiniMapCamera组件MaskLayer属性来改变目标Layer(请不要直接更改相机组件的Culling Mask属性)。
		-您可以修改小地图迷雾颜色(无alpha通道)


2.API
	(1) VisualProvider:
		-你可以通过使用VisualProvider的RemoveVisual()或者直接销毁该组件来取消一个物体的视野。
		-不活跃的GameObject(setActive)将会失去视野，但在单位非常多时，推荐同时使用RemoveVisual以提高性能。
		-你可以使用AddVisual()为单位恢复视野，即使该GameObject不活跃。
		-通过挂载VisualProvider组件使物体拥有视野。
		-你可以通过visualRange属性来动态改变视野大小。
	
	(2) VisualManager:
		-你可以通过VisualManager的SetActive来开启/关闭迷雾。

	(3) VisualTest:
		-您可以通过单位上挂载的该组件的InVisual()方法来判断某单位是否暴露在任何VisualProvider的视野中，当然来自其他模块的诸如隐身一类的特效不在考虑范围之内。
		-您可以通过设置autoTest，并通过isInVisual方法来获得实时数据，但请注意性能损耗。
		-在autoTest关闭时，isInVisual值无意义。


3.已知缺陷
	(1) 过低的分辨率以及精确度会导致糟糕的质量。
	(2) 由于Fog贴图容量有限，无法保存足够信息，导致迷雾的过度以及变换即使经过插值仍然不尽人意，如果性能足够考虑使用多张记录纹理。
	(3) OpenGL ES 的新特性导致无法在老旧的设备及系统上工作。
	(4) 由于使用GPU计算，因而持有视野的单位数量增加基本不会导致性能下降，但在视野单位少时优势不明显，或考虑换用CPU。