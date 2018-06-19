1.Manual
	(1) Get start
		-请为场景添加位于SkillSystem/Perferbs文件夹下预置体SkillManager，并为其添加标签SkillManager。
		-请使用SkillManager的Inspector面板编辑、删除或创建技能。
		-请为需要使用技能的单位添加位于SkillSystem/Script文件夹下的CharactorSkills组件。

	(2) SkillManager
		-您可以通过该组件检视您创建的所有技能，并快速修改该技能的参数对。
		-您可以通过为某一技能增加技能预置体，并在代码中获取它以实现特效。
		-您可以通过该组件快速创建新技能，并编辑其属性字典，您创建的新技能类将位于SkillSystem/SkillClasses文件夹下。

	(3) CharactorSkills
		-您可以通过为单位添加该组件来管理单位的技能。
		-目前通过该组件的Inspector面板，您只能为该单位决定初始拥有的技能。


2.API
	(1) SkillManager
		-aSkill GetInstanceOfSkill<aSkill>(GameObject requester) where aSkill : Skill
			requester 申请技能实例的单位
			通过该方法，您可以得到某一具体技能的实例，该方法较为高效。
		-Skill GetInstanceOfSkillWithString(string skillClassName, GameObject requester)
			skillClassName 技能类名		requester 申请技能实例的单位
			通过该方法，您可以得到某一具体类名方法的实例，该方法较为低效，且类型不安全，请保证您没有使用错误的类名。
		-void StartASkill(Skill skill)
			该方法会在任何单位使用技能时被调用，您可以完善该方法来实现想要的功能。

	(2) CharactorSkills
		-void UseSkill(string skillClassName, Skill.SkillDelegate didStart = null, Skill.SkillDelegate didAction = null, Skill.SkillDelegate didEnd = null)
			skillClassName 方法类名		didStart/didAction/didEnd 回调方法，在技能进行的相应时刻被调用。注意，如果为持续技能，则didAction将被调用多次
			该方法尝试使用一个已经拥有的技能。*
		-void UseSkill<aSkill>(Skill.SkillDelegate didStart = null, Skill.SkillDelegate didAction = null, Skill.SkillDelegate didEnd = null)
			与上一个方法类似。*
		-void AddSkill(string skillClassName)
			通过技能类名添加一个技能。*
		-void AddSkill<aSkill>() where aSkill : Skill
			通过技能类添加一个技能。*
		-void RemoveSkill()
			通过技能类名移除一个技能。*
		-void RemoveSkill<aSkill>() where aSkill : Skill
			通过技能类移除一个技能*
		*注意，以上方法并不会进行合法性检查！

	(3) Skill
		-#！#基于该抽象类，您可以创建具体的技能，但请通过SkillManager面板来创建新子类#！#
		-timeSinceSkillStart     技能从发动以来经过的时间
		-owner                   技能所有者
		-name                    技能名(不是类名，是游戏中想要展示的名字)
		-parameters              可调技能参数的字典
		-resources               资源预置体
		-您必须重写以下方法来完成一个技能：
			protected override bool SkillStart() 该方法返回值若为false，则后续方法将不会再被调用
			protected override bool SkillAction() 该方法返回值若为false，则表示技能动作完成，将进入结束阶段
			protected override void SkillEnd() 该方法在技能结束时调用
			public override string GetDescription() 该方法被设计以返回一个对技能的详细描述。