using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorSkills : MonoBehaviour {

    public List<string> initSkills;

    private Dictionary<string,Skill> skills = new Dictionary<string, Skill>();
    public Dictionary<string, Skill> Skills { get { return skills; }set { skills=value; } }
    private SkillManager sm;

	void Start () {
        sm = GameObject.FindGameObjectWithTag("SkillManager").GetComponent<SkillManager>();
        foreach (var skillClassName in initSkills)
        {
            skills.Add(skillClassName, sm.GetInstanceOfSkillWithString(skillClassName, gameObject));
        }
	}

    public void UseSkill(string skillClassName, Skill.SkillDelegate didStart = null, Skill.SkillDelegate didAction = null, Skill.SkillDelegate didEnd = null)
    {
        StartCoroutine(skills[skillClassName].Execute(didStart, didAction, didEnd));
    }
    public void UseSkill<aSkill>(Skill.SkillDelegate didStart = null, Skill.SkillDelegate didAction = null, Skill.SkillDelegate didEnd = null)
    {
        foreach (var kvPair in skills)
        {
            if (typeof(aSkill) == kvPair.Value.GetType())
            {
                StartCoroutine(kvPair.Value.Execute(didStart, didAction, didEnd));
                return;
            }
        }
    }

    public void AddSkill(string skillClassName)
    {
        skills.Add(skillClassName, sm.GetInstanceOfSkillWithString(skillClassName, gameObject));
    }
    public void AddSkill<aSkill>() where aSkill : Skill, new()
    {
        skills.Add(typeof(aSkill).ToString(), sm.GetInstanceOfSkill<aSkill>(gameObject));
    }
    public void RemoveSkill(string skillClassName)
    {
        skills.Remove(skillClassName);
    }
    public void RemoveSkill<aSkill>() where aSkill : Skill
    {
        foreach (var kvPair in skills)
        {
            if (typeof(aSkill) == kvPair.Value.GetType())
            {
                skills.Remove(kvPair.Key);
                return;
            }
        }
    }
}
