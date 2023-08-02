using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
public enum BattleAction{RunSkill, SwitchPokemon, ThrowBall, OtherItem, Run}//选择状态
public class BattleSystem : MonoBehaviour
{
	private enum BState { RunningTurn, BUSY, BattleOver };//战斗状态
	private enum ButtonChoose{ Selection, Learn, DontLearn };//选择提示状态

    [Header("己方")]
	[SerializeField] BattleUnit playerUnit;
	[Header("敌方")]
	[SerializeField] BattleUnit enemyUnit;
	[Header("战斗背景管理")]
	[SerializeField] BattleBackGround backGround;
	[Header("战斗文本")]
	[SerializeField] BattleDialogBox dialogBox;
	[SerializeField] ChoosePanel choosePanel;
    [Header("训练家")]
	[SerializeField] Transform playerAnimatorTrans;
	[SerializeField] Transform trainerAnimatorTrans;
	[SerializeField] TrainerAnimator playerSprites;
	[SerializeField] TrainerAnimator enemySprites;
	[Header("投掷时的精灵球")]
	[SerializeField] GameObject pokeballObj;
	private ThrowBall ball;
	[SerializeField] EvoPanel evoPanel;
	private BState state;
	private ButtonChoose buttonState;
	StringBuilder combatText = new StringBuilder();
#region 记录的值
    //精灵球
	private GameObject clonePokeball;
    //对战精灵
	private PokemonTeam playerTeam; private Pokemon wildPokemon;
	private PlayerMovement player;  private TrainerCtrller trainer;
	private int currentSkill;
	/// <summary>
	/// 学习技能按键值
	/// </summary>
	private int learnNum = 0;
	/// <summary>
	/// 当前在场宝可梦在背包的位置
	/// </summary>
	private int currentPkmNum;
	/// <summary>
	/// 使用的道具
	/// </summary>
	private ItemBase item;
	private bool isTrainerBattle;
	private bool isFaintedSwitch;
#endregion
#region 单次战斗数据,每场重置
	/// <summary>
	/// 记录回合数
	/// </summary>
	private int times = 1;
	private int escapeAttempts;//逃跑基数
	private bool isTrainerSwitch;
	private bool isMega; private int megaNum;//mega
	private bool isDynamax; private int dynamaxTurnNum;//极巨化
	public event Action<bool> OnBattleOver;
#endregion
    private void Start()
	{
		choosePanel.ChooseSkillPanel.learnSkillAction += LearnSkill;
		UIManager.Instance.ItemHandler.UseItemInBattleAction += UseItem;
	}
#region 加载战斗
    /// <summary>
    /// 与野生宝可梦战斗
    /// </summary>
    public void StartWildBattle(PlayerMovement _player, PokemonTeam _playerTeam, Pokemon _wildPokemon)
    {
        isTrainerBattle = false;
        //设置玩家和野生精灵信息
        playerTeam = _playerTeam;
        wildPokemon = _wildPokemon;
        player = _player;
		SetupBattle();
    }

    /// <summary>
    /// 与训练家战斗
    /// </summary>
    public void StartTrainerBattle(PlayerMovement _player, PokemonTeam _playerTeam, TrainerCtrller _trainer)
    {
        isTrainerBattle = true;
        //设置玩家与训练家信息
        playerTeam = _playerTeam;
        trainer = _trainer;
        player = _player;
		SetupBattle();
    }

	private async void SetupBattle()
    {
		await UniTask.Delay(450);
		gameObject.SetActive(true);

        //准备战斗//关掉Hud
        playerUnit.ClearHud();
        enemyUnit.ClearHud();

        //判断是否为野外对战
        if(!isTrainerBattle)
        {
            playerUnit.SetData(playerTeam.GetHealthyPokemon(), false);
			enemyUnit.SetData(wildPokemon, false);

				combatText.Append("一只野生的"); combatText.Append(enemyUnit.Pokemon.NickName);
			    combatText.Append("出现了!"); await ShowCombatText();

			dialogBox.SetSkillData(playerUnit.Pokemon.Skill, wildPokemon.Base.Type1, wildPokemon.Base.Type2);
        }
        else
        {
            //显示人物
            playerSprites._Active(playerAnimatorTrans.position);

			enemySprites._Active(trainerAnimatorTrans.position);
			await UniTask.Delay(500);
			await enemySprites.PlayNormalAnimator();

                combatText.Append(trainer.TrainerName); combatText.Append("发起了战斗!");
                await ShowCombatText();

            //敌方生成精灵
			SetPokeBall(enemySprites.transform.position);
			//敌方人物退场
			enemySprites.NpcOut();
		    //敌方扔球动画
		    await ball.ReleaseThePokemon(enemyUnit.transform.position, 0.5f);

            Pokemon enemyPokemon = trainer.GetHealthyPokemon();

            //设置敌方面板
			enemyUnit.SetData(enemyPokemon, false);

                combatText.Append(trainer.TrainerName); combatText.Append("派出了");
			    combatText.Append(enemyPokemon.NickName); combatText.Append("!");
                await ShowCombatText();

            //我方生成精灵
			//人物左移
			playerSprites.PlayerOut();
			//人物扔球动作动画
			await playerSprites.PlayPlayerThrowingBallAnim();

			//移动球位置
			SetPokeBall(playerSprites.transform.position);
		    //扔球动画
		    await ball.ReleaseThePokemon(playerUnit.transform.position);

            Pokemon playerPokemon = playerTeam.GetHealthyPokemon();

            playerUnit.SetData(playerPokemon, false);

                combatText.Append("去吧"); combatText.Append(playerPokemon.NickName);
				combatText.Append("!"); await ShowCombatText();

            dialogBox.SetSkillData(playerUnit.Pokemon.Skill, enemyPokemon.Base.Type1, enemyPokemon.Base.Type2);
        }
        currentPkmNum = playerTeam.CurrentBattlePokemon;//当前己方宝可梦在背包的位置
        escapeAttempts = 0;//逃跑基数
        PlayerReady();
    }
#endregion
#region 玩家准备回合
    /// <summary>
    /// 玩家准备回合
    /// </summary>
	private async void PlayerReady()
    {
        if (isDynamax)//检查是否极巨化过
		{
			dynamaxTurnNum++;//计算极巨化回合数
			if (dynamaxTurnNum > 2)
			{
				playerUnit.Pokemon.EndDynamax();//结束极巨化
				if (playerUnit.Pokemon.Base.CanGigantamax)
				{
					await playerUnit.AnimStage.EndGigantamax(playerUnit.ChangeSprite);
				}
				else
				{
					playerUnit.AnimStage.EndDynamaxAnim();
				}
				isDynamax = false;

                    combatText.Append(playerUnit.Pokemon.NickName);combatText.Append("变回原样了!");
                    await ShowCombatText();
			}
		}

		await dialogBox.TypeDialog("要做什么呢?");

		//玩家准备
		dialogBox.OpenChoosePanel();
    }

	/// <summary>
	/// 宝可梦出场事件处理
	/// </summary>
	public async UniTask SetAppearancesEvent(AbilityBase abilityBase)
	{
		if(abilityBase != null && abilityBase.TriggerType == Ability_TriggerType.Appearances)
		{
			WeatherType weather;
			(abilityBase as AppearancesAbility).CheckAppearancesEvent(enemyUnit.Pokemon, out weather);
		}
		await UniTask.Yield();
	}

	/// <summary>
	/// 回合结束事件处理
	/// </summary>
	public async UniTask TurnOver(AbilityBase abilityBase)
	{
		times++;
		//
		playerUnit.Pokemon.ResetAbilityState();
		enemyUnit.Pokemon.ResetAbilityState();
		await UniTask.Yield();
	}
#endregion
#region 回合事件处理
    private async void RunTurns(BattleAction playerAction)
	{
		//关掉所有按键
		dialogBox.CloseButton();

		state = BState.RunningTurn;

		if(playerAction == BattleAction.RunSkill)//如果玩家选择释放技能
		{
			playerUnit.Pokemon.CurrentSkill = playerUnit.Pokemon.Skill[currentSkill];
			enemyUnit.Pokemon.CurrentSkill = enemyUnit.Pokemon.GetRandomSkills();

			//检查技能优先级
			byte playerPriority = playerUnit.Pokemon.CurrentSkill.Base.Priority;
			byte enemyPriority = enemyUnit.Pokemon.CurrentSkill.Base.Priority;

			bool playerFirst =
			     enemyPriority == playerPriority? (playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed) :
				 enemyPriority >  playerPriority? false : true;//优先级相同比速度

			//按顺序放技能
			BattleUnit firstUnit;
			BattleUnit secondUnit;
			if(playerFirst)
			{
				firstUnit = playerUnit;
				secondUnit = enemyUnit;
			}
			else
			{
				firstUnit = enemyUnit;
				secondUnit = playerUnit;
			}

			Pokemon secondPokemon = secondUnit.Pokemon;

			//首回合释放技能
			await RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentSkill);
			if(state == BState.BattleOver)
			{
				return;
			}

			if(secondPokemon.isActive)//如果后出手的宝可梦还存活 释放技能
			{
				//次回合释放技能
				await UniTask.Delay(200);//暂时 怕前一个技能没跑完
				await RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentSkill);
				if(state == BState.BattleOver)
				{
					return;
				}

				//异常状态处理
				if(firstUnit.Pokemon.Status != null)
				{
					await RunAfterTurn(firstUnit);
					if(state == BState.BattleOver)//判断异常状态濒死结束
					{
						return;
					}
				}
				if(secondUnit.Pokemon.Status != null)
				{
					await RunAfterTurn(secondUnit);
					if(state == BState.BattleOver)//判断异常状态濒死结束
					{
						return;
					}
				}
			}
		}
		else//其它选择
		{
			switch(playerAction)
			{
			    //替换
				case BattleAction.SwitchPokemon:

				    await SwitchPokemon();

				break;

				//扔球
				case BattleAction.ThrowBall:

					await ThrowPokeBall();

				break;

				//其它道具//治疗 状态 PP 复活
				case BattleAction.OtherItem:

					//等血量更新动画
					await UniTask.Delay(500);
					//pp面板值没更新
					await ShowCombatText();

				break;

			    //逃跑
				case BattleAction.Run:

                    await TryToEscape();

				break;
			}
			if(state == BState.BattleOver)
			{
				return;
			}

			//选择完成之后, 对方需要进行一次攻击
			Skill skill = enemyUnit.Pokemon.GetRandomSkills();
			state = BState.RunningTurn;
			await RunMove(enemyUnit, playerUnit, skill);
			if(state == BState.BattleOver)
			{
				return;
			}
			if(enemyUnit.Pokemon.Status != null)
			{
				//Debug.Log("我方选择后敌方异常状态扣血");
				await RunAfterTurn(enemyUnit);
				//yield return enemyUnit.Hud.UpdateHP();
				await UniTask.Delay(500);
				if (state == BState.BattleOver)
				{
					return;
				}
			}
		}
		if(state != BState.BattleOver && !isFaintedSwitch)
		{
			PlayerReady();
		}
	}
#endregion
#region 释放技能
    /// <summary>
    /// 释放技能
    /// </summary>
    private async UniTask RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Skill skill)
	{
		Pokemon sourcePokemon = sourceUnit.Pokemon;
		Pokemon targetPokemon = targetUnit.Pokemon;
		bool isSourceAbilityActive = sourcePokemon.AbilityWasActive;//为空判断为已触发
		bool isTargetAbilityActive = targetPokemon.AbilityWasActive;

		bool canRunMove = sourcePokemon.OnBeforeMove();//检查是否能行动
		await ShowQueueString(sourcePokemon.StatusChange);

		//不能动
		if(!canRunMove)
		{
			await sourceUnit.AnimStage.ConditionColor(sourcePokemon.Status);//状态动画
			await ShowQueueString(sourcePokemon.StatusChange);
			//yield return sourceUnit.Hud.UpdateHP();//混乱扣血
			await UniTask.Delay(500);

			if(sourcePokemon.isFainted)
			{
				await HandlePokemonFainted(sourceUnit);
			}
			return;
		}

		skill.PPValueChange(-1);
		if (sourceUnit == playerUnit)//如果是玩家, 就更新技能栏
		{
			dialogBox.RefreshSkillData(currentSkill, playerUnit.Pokemon.Skill[currentSkill], targetPokemon.Base.Type1, targetPokemon.Base.Type2);
		}

		    //释放技能文本后不用延时1s//但没写
		    combatText.Append(sourcePokemon.NickName); combatText.Append("使用了");
		    combatText.Append(skill.Base.SkillName); await ShowCombatText();

		if (CheckIfSkillHits(skill, sourcePokemon, targetPokemon))//检查技能命中
		{
			if (skill.Base.Category == SkillCategory.Status)//状态类技能
			{
				if(skill.Base.AnimatorType == SkillAnimatorType.Particle && skill.Base.ParticleAnim != null)
				{
					Vector3 startPos = sourceUnit.transform.position;
					//startPos.y += (int)sourcePokemon.Base.Physique;
					startPos.y += 1.2f;
					GameObject clone = Instantiate
					(
						skill.Base.ParticleAnim.gameObject,
						startPos,
						Quaternion.identity
					);

					Vector3 targetPos = targetUnit.transform.position;
					targetPos.y ++;
					clone.GetComponent<SkillParticleHandler>().RunMove(targetPos);
					await UniTask.Delay(1500);
				}
				await RunMoveEffects(skill.Base.Effects, sourcePokemon, targetPokemon);
			}
			else
			{
				//突进动画记录值(暂时)
				bool phySkill = skill.Base.Category == SkillCategory.Physical;
				//技能释放动画
				if(phySkill)
				{
					await sourceUnit.AnimStage.AttackAnim(sourceUnit.IsPlayerUnit);
				}

				SkillAnimatorType skillAnimatorType = skill.Base.AnimatorType;
				switch(skillAnimatorType)
				{
					//暂无动画 打击星星
					case SkillAnimatorType.None:

					    Instantiate(Resources.Load<GameObject>("Skill/StarBurst2D"), targetUnit.transform.position, Quaternion.identity);

			        break;

					//动图
					case SkillAnimatorType.Image:

					    switch (skill.Base.Target)
					    {
					        case SkillTarget.Foe:
					    	    await targetUnit.AnimStage.SingleAnimate(skill.Base.Sid);
					    	break;

					        case SkillTarget.Self:
					    	    await sourceUnit.AnimStage.SingleAnimate(skill.Base.Sid);
					    	break;
					    }

					break;

					//粒子
					case SkillAnimatorType.Particle:

						Vector3 startPos = sourceUnit.transform.position;
						//startPos.y += (int)sourcePokemon.Base.Physique;
						startPos.y += 1.2f;
						GameObject clone = Instantiate
						(
							skill.Base.ParticleAnim.gameObject,
							startPos,
							Quaternion.identity
						);

						Vector3 targetPos = targetUnit.transform.position;
						//PokemonBodyType bodyType = targetUnit.Pokemon.Base.BodyType;
						//float fixY = bodyType == PokemonBodyType.Small? 1f : bodyType == PokemonBodyType.Medium? 2f : 3f;
						targetPos.y ++;
						clone.GetComponent<SkillParticleHandler>().RunMove(targetPos);
						await UniTask.Delay(1500);

					break;
				}

				if(phySkill)
				{
					await sourceUnit.AnimStage.EndAttackAnim(sourceUnit.IsPlayerUnit);
				}

				targetUnit.AnimStage.InjuredColor();//受伤动画

				//伤害细节
				await ShowQueueString(targetPokemon.TakeDamage(skill, sourcePokemon));
				await UniTask.Delay(500);//原本是等血条的

				targetUnit.AnimStage.NormalColor();//恢复

				if(skill.Base.AddOn && skill.Base.Category != SkillCategory.Status)
				{
					bool isActive = !isSourceAbilityActive &&
							        sourcePokemon.Ability.TriggerType == Ability_TriggerType.Attack &&
							        (sourcePokemon.Ability as AttackAbility).CheckAddOnPercentUp();
					if(isActive)
					{
						sourcePokemon.ChangeAbilityState(true);
					}

                    //追加技能
				    if(skill.Base.IsAddOnValid(isActive))
				    {
						SkillEffects effects = skill.Base.Effects;
						//如果目标是对方要判断对方是否存活(我方释放技能即为存活)
				    	if(effects.Target == SkillTarget.Foe)
				    	{
				    		if(targetPokemon.isActive)//保证存活才触发
				    		{
				    			await RunMoveEffects(effects, sourcePokemon, targetPokemon);
				    		}
				    	}
				    	else
				        {
				    		await RunMoveEffects(effects, sourcePokemon, targetPokemon);
				    	}
				    }
				}

				//如果宝可梦特性为攻击后概率使敌方异常状态
				if(!isSourceAbilityActive)
				{
					//保证存活才触发
					if(targetPokemon.isActive && sourcePokemon.Ability.TriggerType == Ability_TriggerType.Attack)
					{
						//如果触发了
						if((sourcePokemon.Ability as AttackAbility).CheckAbilityAddOnCondition(ref targetPokemon))
						{
							sourcePokemon.ChangeAbilityState(isSourceAbilityActive = true);
							await ShowQueueString(targetPokemon.StatusChange);
						}
					}
				}
			}

			if (targetPokemon.isFainted)//检查宝可梦存活情况
			{
				//特性处理
				if(targetPokemon.Ability != null && skill.Base.SkillType == SkillType.接触类)//技能是接触类 没写**********
				{
				    if(targetPokemon.Ability.TriggerType == Ability_TriggerType.Touch)//如果是反伤
					{
					    //处理反伤效果
						string message = (targetPokemon.Ability as TouchAbility).CheckThorn(ref sourcePokemon);
                        if(message == null)
						{
							//状态反伤或没反伤
							//处理状态
						    await RunAfterTurn(sourceUnit);
						}
						else
						{
							//血量反伤
							await dialogBox.TypeDialog(message);
							if(sourcePokemon.isFainted)
							{
								await HandlePokemonFainted(sourceUnit);
								if (state == BState.BattleOver)
			                    {
			                    	return;
		    	                }
							}
						}
					}
				}

				await HandlePokemonFainted(targetUnit);
				if (state == BState.BattleOver)
			    {
			    	return;
		    	}
				if (isTrainerSwitch)
				{
					await UniTask.Delay(1500);
					isTrainerSwitch = false;
				}
			}
		}
		else//闪避
		{
			//target闪避一下//动画没写
			combatText.Append(sourcePokemon.NickName); combatText.Append("没有命中目标!");
            await ShowCombatText();
		}
	}

    /// <summary>
    /// 技能效果
    /// </summary>
    private async UniTask RunMoveEffects(SkillEffects effects, Pokemon source, Pokemon target)
	{
		StatBoost[] statBoosts = effects.Boosts;
		if (statBoosts != null)//临时增益等级
		{
			if (effects.Target == SkillTarget.Self)
			{
				source.ApplyBoosts(statBoosts);
				if(source == playerUnit.Pokemon)
				{
					int count = statBoosts.Length;
					if(count > 1)
					{
						playerUnit.AnimStage.BuffEffects(2);
						AudioManager.Instance.StatChangeAudio(true);
					}
					else if(count < 2 && statBoosts[0].Boost > 1)
					{
                        playerUnit.AnimStage.BuffEffects(0);
						AudioManager.Instance.StatChangeAudio(true);
					}
					else if(count < 2 && statBoosts[0].Boost < 0)
					{
						playerUnit.AnimStage.BuffEffects(1);
						AudioManager.Instance.StatChangeAudio(false);
					}
				}
			}
			else //if(effects.Target == SkillTarget.Foe)
			{
				if(target.Ability != null && target.Ability.TriggerType == Ability_TriggerType.Defence)
			    {
			        //检查是否有不会降低能力的特性
			    	string message = (target.Ability as DefenceAbility).CheckBoostsDownDefence(ref statBoosts);
			    	if(message != null)
			    	{
						//不降低能力
			    		await dialogBox.TypeDialog(message);
			    	}
					else
			    	{
			    		target.ApplyBoosts(statBoosts);
				        if(target == playerUnit.Pokemon && statBoosts.Length != 0 && statBoosts[0].Boost < 0)
				        {
				        	playerUnit.AnimStage.BuffEffects(1);
				        	AudioManager.Instance.StatChangeAudio(false);
				        }
			    	}
			    }
				else
				{
				    target.ApplyBoosts(statBoosts);
				    if(target == playerUnit.Pokemon && statBoosts.Length != 0 && statBoosts[0].Boost < 0)
				    {
				    	playerUnit.AnimStage.BuffEffects(1);
				    	AudioManager.Instance.StatChangeAudio(false);
				    }
				}

			}
		}

		//异常状态
		if(effects.Status != 0)
		{
			target.SetStatus(effects.Status);
		}
		//不稳定的状态
		if (effects.VolatileStatus != 0)
		{
			target.SetVolatileStatus(effects.VolatileStatus);
		}

		//异常状态文本细节
		await ShowQueueString(source.StatusChange);
		await ShowQueueString(target.StatusChange);
	}

    /// <summary>
    /// 异常状态
    /// </summary>
    private async UniTask RunAfterTurn(BattleUnit sourceUnit)
	{
		sourceUnit.Pokemon.OnAfterTurn();

		ConditionID conditionID = sourceUnit.Pokemon.Status.ConditionID;
		if//麻痹 睡觉 冰冻 (畏缩) 不显示(技能放完后处理状态)
	    (
			conditionID == ConditionID.psn || conditionID == ConditionID.hyp ||
			conditionID == ConditionID.brn
		)
		{
			await sourceUnit.AnimStage.ConditionColor(sourceUnit.Pokemon.Status);//状态颜色动画
		}

		//异常状态文本
		await ShowQueueString(sourceUnit.Pokemon.StatusChange);
		//yield return sourceUnit.Hud.UpdateHP();
		await UniTask.Delay(500);
		if (sourceUnit.Pokemon.isFainted)
		{
			await HandlePokemonFainted(sourceUnit);
			if (state == BState.BattleOver)
			{
				return;
			}
			await UniTask.WaitUntil(IsRunningTurnState);
		}
	}
	private bool IsRunningTurnState() => state == BState.RunningTurn;
#endregion
#region 命中计算
    /// <summary>
    /// 技能命中计算
    /// </summary>
    private bool CheckIfSkillHits(Skill skill, Pokemon source, Pokemon target)
	{
        //命中 = (255*技能命中率 * 道具修正 * 场上状态 * 天气修正) / 255;
        if(skill.Base.AlwaysHits)//判断永远命中和一击必杀
        {
            return true;
        }
        int B = Mathf.FloorToInt(255 * skill.Base.Accuracy);//招式命中*255向下取整
        float E = 1;//道具修正 例如光粉0.9f
        float F = 1;
        int boostLevel = source.StatBoosts[Stat.Accuracy] - target.StatBoosts[Stat.Evasion];
        switch(boostLevel)
        {
            //case  0: break;
            case -6: F = 0.33f; break; case -5: F = 0.38f; break;
            case -4: F = 0.43f; break; case -3: F = 0.5f ; break;
            case -2: F = 0.6f ; break; case -1: F = 0.75f; break;
            case  1: F = 1.33f; break; case  2: F = 1.67f; break;
            case  3: F = 2f   ; break; case  4: F = 2.33f; break;
            case  5: F = 2.67f; break; case  6: F = 3f   ; break;
        }
        float G = 1;//天气修正 复眼1.3f 雪隐 0.8f 沙隐 0.8f 活力 物理招式 0.8f
        int A = (int)(((B * E * F * G) / 255f) * 100);

        return UnityEngine.Random.Range(0, 100) < A;
	}
#endregion
#region 战斗结束判断debug
	/// <summary>
    /// 宝可梦倒下后事件处理
    /// </summary>
    private async UniTask HandlePokemonFainted(BattleUnit faintedUnit)
	{
		state = BState.BUSY;

		Pokemon fainted = faintedUnit.Pokemon;

		    combatText.Append(fainted.NickName);combatText.Append("倒下了");
		    await ShowCombatText();

		await faintedUnit.AnimStage.Fainted();//宝可梦倒下动画

		if (!faintedUnit.IsPlayerUnit)//己方获得经验
		{
			Pokemon playerPokemon = playerUnit.Pokemon;
			if(!playerPokemon.BasePointsWasMax)//总值没到510加努力值
			{
				List<BasePoint> basePoints = fainted.Base.EValue;
				int p = 0;//加值位置
				foreach(BasePoint point in basePoints)
				{
					p = (int) point.Type;//努力值Type
					playerPokemon.GetBasePoint(p, point.Value);//Mathf.Clamp不用判断满没满
					if(playerPokemon.BasePointsWasMax)
					{
						break;
					}
				}
			}

			if(playerPokemon.Level < 100)//经验计算
			{
				int expYield = fainted.Base.ExpYield;//经验基数
           	 	int enemyLevel = fainted.Level;
	            //int s = 0;//参加对战且不处于濒死状态的宝可梦
            	float trainerBonus = isTrainerBattle? 1.5f : 1f;//经验加成
           	 	int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
          		playerPokemon.Exp += expGain;//获得经验

				    combatText.Append(playerPokemon.NickName); combatText.Append("获得了");
				    combatText.Append(expGain); combatText.Append("点经验!");
		   	        await ShowCombatText();

				await playerUnit.Hud.SetExpSmooth();//经验条动画
				while (playerPokemon.CheckForLevelUp())//升级
				{
					playerUnit.Hud.SetLevel();

					    combatText.Append(playerPokemon.NickName); combatText.Append("升到了");
			    	    combatText.Append(playerPokemon.Level); combatText.Append("级!");
		        	    await ShowCombatText();

					playerPokemon.OnLevelChanged = true;
					//检查进化
					Evolution evolution = playerPokemon.Base.Evolution;
					if(evolution.CanEvolution && evolution.EvoLevel == playerPokemon.Level)
					{
						//yield return evoPanel.EvolutionAnim(playerUnit.Pokemon);
						//playerUnit.Pokemon.Evolution();
						await UniTask.Delay(1000);
					}

					//检查是否可以学习技能
					LearnableSkill newSkill = playerPokemon.GetLearnSkillAtCurrentLevel();
					if (newSkill != null)
					{
						if(playerPokemon.Skill.Count < 4)
						{
							playerPokemon.LearnSkill(newSkill.Base);

							    combatText.Append(playerPokemon.NickName); combatText.Append("学会了");
			      	 	        combatText.Append(newSkill.Base.SkillName); combatText.Append("!");
                   	            await ShowCombatText();
						}
						else
						{
							    combatText.Append(playerPokemon.NickName); combatText.Append("想要学会新技能就要忘掉一个技能!");
							    await ShowCombatText();

							await LearnPanel(playerPokemon, newSkill.Base);
						}
						dialogBox.SetSkillData(playerPokemon.Skill, enemyUnit.Pokemon.Base.Type1, enemyUnit.Pokemon.Base.Type2);//更新技能栏
					}
					await playerUnit.Hud.SetExpSmooth(true);
				}
				playerPokemon.LevelUpUpdate();
			}
			await UniTask.Delay(1000);
		}

		if (faintedUnit.IsPlayerUnit)//如果是己方宝可梦
		{
			if (playerTeam.GetHealthyPokemon() != null)
			{
				isFaintedSwitch = true;
				if (isTrainerBattle)
				{
					choosePanel.SetDataForSwitch(playerTeam.Pokemons, true, currentPkmNum);
				}
				else
				{
					dialogBox.ContinueBattle(ContinueBattle);
				}
			}
			else//没有存活宝可梦 结束战斗
            {
				await ExitBattle();
				return;
            }
		}
		else
		{
			if (!isTrainerBattle)//不是玩家战斗 即结束战斗
		    {
				await dialogBox.TypeDialog("战胜了对手!");
		    	BattleOver(true);
		    	playerUnit.ResetSR();
		    }
		    else
		    {
		    	Pokemon healthyPokemon = trainer.GetHealthyPokemon();
		    	if (healthyPokemon != null)
		    	{
		    		isTrainerSwitch = true;
					await NextTrainerPokemon(healthyPokemon);
		    	}
		    	else
		    	{
					await dialogBox.TypeDialog("战胜了对手!");
					    combatText.Append(trainer.TrainerName); combatText.Append("看来还要继续学习啊...");
					    await ShowCombatText();

		    		BattleOver(true);
			    	playerUnit.ResetSR();
		    	}
		    }
		}
	}

	/// <summary>
	/// 战斗结束
	/// </summary>
	/// <param name="won"></param>
	private void BattleOver(bool won)
	{
		state = BState.BattleOver;
		isDynamax = false;
		isMega = false;
		dialogBox.EndBattle();
		if(trainer != null)
		{
			trainer.BattleLost();
			trainer = null;
		}
		isTrainerSwitch = false;
		dynamaxTurnNum = 0;
		playerUnit.AnimStage.StopCloud();
		playerTeam.EndBattle();
		this.OnBattleOver(won);
	}
	private async UniTask ExitBattle()
	{
		await dialogBox.TypeDialog("被击败了...");
		enemyUnit.ResetSR();
		BattleOver(true);
	}
#endregion
#region 替换宝可梦debug
    private async UniTask NextTrainerPokemon(Pokemon nextPokemon)
	{
		state = BState.BUSY;

		//扔球出场动画
		SetPokeBall(enemySprites.transform.position);
		await ball.ReleaseThePokemon(enemyUnit.transform.position);

		enemyUnit.SetData(nextPokemon, false);

		combatText.Append(trainer.TrainerName); combatText.Append("派出了");
		combatText.Append(nextPokemon.NickName); combatText.Append("!");
		await ShowCombatText();

		state = BState.RunningTurn;
	}

	/// <summary>
	/// 替换精灵
	/// </summary>
	private async UniTask SwitchPokemon()
	{
		state = BState.BUSY;
		int selectPokemon = choosePanel.ChoosePos;
		playerTeam.Pokemons[currentPkmNum].InsteadCure();//替换属性恢复
		currentPkmNum = selectPokemon;//记录

		if(playerUnit.Pokemon.Dynamax)//清除极巨化
		{
			playerUnit.Pokemon.EndDynamax();
			isDynamax = false;
		}

		    combatText.Append("回来吧"); combatText.Append(playerUnit.Pokemon.NickName);
			combatText.Append("!"); await ShowCombatText();

		//替换动画
		if(!isFaintedSwitch)
		{
			await playerUnit.AnimStage.ZoomInPokemonAnim();
		}

		//扔球出宝可梦
		Vector3 playerUnitPos = playerUnit.transform.position;
		Vector3 ballStartPos = playerUnitPos;
		ballStartPos.x -= 10f;
		ballStartPos.y += 3f;
		SetPokeBall(ballStartPos);
		//扔球动画
		await ball.ReleaseThePokemon(playerUnitPos);


		Pokemon selectedPokemon = playerTeam.Pokemons[selectPokemon];
		playerUnit.SetData(selectedPokemon, isMega && megaNum == selectPokemon);

		    combatText.Append("去吧"); combatText.Append(selectedPokemon.NickName);
			combatText.Append("!"); await ShowCombatText();

		dialogBox.SetSkillData(playerUnit.Pokemon.Skill, enemyUnit.Pokemon.Base.Type1, enemyUnit.Pokemon.Base.Type2);//更新技能栏
		state = BState.RunningTurn;

		if (isFaintedSwitch)
		{
			PlayerReady();
			isFaintedSwitch = false;
		}
	}
#endregion
#region 捕捉宝可梦
    /// <summary>
    /// 投掷精灵球
    /// </summary>
    /// <returns></returns>
    private async UniTask ThrowPokeBall()
	{
		state = BState.BUSY;
		if(isTrainerBattle)//不能抓其它训练家的宝可梦
		{
			await dialogBox.TypeDialog("人不能, 至少不应该...");
			state = BState.RunningTurn;
			return;
		}

		    combatText.Append(player.PlayerName); combatText.Append("投掷了");
		    combatText.Append(item.ItemName); combatText.Append("!");
		    await ShowCombatText();

		//球设置
		Vector3 startPos = playerUnit.transform.position;
		startPos.x -= 7f;
		startPos.y += 5f;
		SetPokeBall(startPos);

		//动画
		await ball.ThrowBallAnim(enemyUnit.transform.position, item.ID);
		await enemyUnit.AnimStage.NarrowPokemonAnim();//缩小动画
		await ball.CloseBall();

		//处理捕捉数据
		Pokemon enemyPokemon = enemyUnit.Pokemon;
		//计算球的捕捉系数
		float catchRate = (item as PokeBall).CatchRate(times, playerUnit.Pokemon, enemyPokemon);

		int shakeCounts = TryToCatchPokemon(enemyPokemon, catchRate);

		for(int i = Mathf.Min(shakeCounts, 3); i > 0; i--)
		{
			await UniTask.Delay(500);
			await ball.ShakeThis();
		}

		await UniTask.Delay(500);
		if(shakeCounts == 4)
		{
			ball.CatchPokemon();

			    combatText.Append("抓到了"); combatText.Append(enemyPokemon.NickName);
			    combatText.Append("!"); await ShowCombatText();

            playerTeam.AddPokemon(enemyPokemon);

			    combatText.Append(enemyPokemon.NickName);
			    if(playerTeam.Pokemons.Length < 6)
			    {
			    	combatText.Append("放到了队伍中!");
			    }
			    else
			    {
			    	combatText.Append("放到了箱子中!");
			    }
			    await ShowCombatText();

			BattleOver(true);
			enemyUnit.AnimStage.ResetColorAndTrans();
			clonePokeball.SetActive(false);
			ball.Normal();
			playerUnit.ResetSR();
			enemyUnit.ResetSR();
		}
		else
		{
			AudioManager.Instance.BallBreakAudio();
			clonePokeball.SetActive(false);
			enemyUnit.AnimStage.ResetColorAndTrans();

			if(shakeCounts < 2)
			{
				combatText.Append(enemyPokemon.NickName);combatText.Append("挣脱了!");
				await ShowCombatText();
			}
			else
			{
				await dialogBox.TypeDialog("这都没抓到!!!!!");
			}

			state = BState.RunningTurn;
			if(playerUnit.Pokemon.Status != null)
			{
				await RunAfterTurn(playerUnit);
				if (state == BState.BattleOver)
				{
					return;
				}
			}
		}
	}

	/// <summary>
	/// 捕捉摇动公式
	/// </summary>
	/// <param name="pokemon">捕捉的宝可梦</param>
	/// <param name="ballRate">球的捕获加成</param>
	/// <returns>返回摇动次数</returns>
	int TryToCatchPokemon(Pokemon pokemon, float ballRate)
    {
        float B = (((3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * ballRate) / (3 * pokemon.MaxHP)) * AllConditionData.GetStatusBonus(pokemon.Status);

        if(B >= 255)//直接捕获
        {
            return 4;
        }

		int G = (int) (65536 / Mathf.Pow((255f / B), 0.1875f));

		int shakeCounts = 0;
		for(int i = 0; i < 4; ++i)
		{
			if(UnityEngine.Random.Range(0, 65535) < G)
            {
				++shakeCounts;
            }
			else
			{
				break;
			}
		}
        return shakeCounts;
    }
#endregion
#region 球生成和位置调整
    /// <summary>
    /// 球生成和位置调整
    /// </summary>
    /// <param name="pos">生成/移动到位置</param>
    private void SetPokeBall(Vector3 pos)
	{
		//宝可梦球预制体缓存实例化
		if(System.Object.ReferenceEquals(clonePokeball, null))
		{
			//没有就实例化
			clonePokeball = Instantiate(pokeballObj, pos, Quaternion.identity);
			ball = clonePokeball.GetComponent<ThrowBall>();
		}
		else
		{
			//有就调整位置
			clonePokeball.transform.position = pos;
		}
	}
#endregion
#region 逃跑debug
    /// <summary>
    /// 逃跑
    /// </summary>
    /// <returns></returns>
    private async UniTask TryToEscape()
	{
		state = BState.BUSY;

		if (isTrainerBattle)//与训练家对战不能逃跑
		{
			await dialogBox.TypeDialog("不能退缩!");
			state = BState.RunningTurn;
			return;
		}

		escapeAttempts++;//逃跑基数
		int playerSpeed = playerUnit.Pokemon.Speed;
		int enemySpeed = enemyUnit.Pokemon.Speed;
		if (enemySpeed < playerSpeed)//对比速度
		{
			await dialogBox.TypeDialog("逃跑成功!");
			BattleOver(true);
			AudioManager.Instance.RunSound();
			playerUnit.ResetSR();
			enemyUnit.ResetSR();
		}
		else
		{
			float f = playerSpeed * 128 / enemySpeed + 30 * escapeAttempts;
			f %= 256f;
			if ((float)UnityEngine.Random.Range(0, 256) < f)
			{
				await dialogBox.TypeDialog("逃跑成功!");
				BattleOver(true);
				AudioManager.Instance.RunSound();
				playerUnit.ResetSR();
				enemyUnit.ResetSR();
			}
			else
			{
				await dialogBox.TypeDialog("逃跑失败!");
				state = BState.RunningTurn;
				if (playerUnit.Pokemon.Status != null)
				{
					await RunAfterTurn(playerUnit);
					if (state == BState.BattleOver)
					{
						return;
					}
				}
			}
		}
	}
#endregion
#region 战斗中进化
    private enum BattleEvoSelection{ Mega, Dynamax, Gigantamax }
    public void MegaButton()
	{
		if (!playerUnit.Pokemon.Dynamax && playerUnit.Pokemon.CanMega())
		{
			Evolution(BattleEvoSelection.Mega);
			megaNum = currentPkmNum;
			isMega = true;
			dialogBox.Mega();
		}
	}

	public void DynamaxButton()
	{
		if (!playerUnit.Pokemon.Mega && !isDynamax)
		{
			isDynamax = true;
			Evolution(playerUnit.Pokemon.Base.CanGigantamax? BattleEvoSelection.Gigantamax : BattleEvoSelection.Dynamax);
			dialogBox.Dynamax();
		}
	}

    private async void Evolution(BattleEvoSelection selection)
	{
		dialogBox.CloseButton();
		    combatText.Append(playerUnit.Pokemon.NickName);
		switch (selection)
		{
		    case BattleEvoSelection.Mega:

					combatText.Append("Mega进化了!");
			        await ShowCombatText();

			    await playerUnit.AnimStage.MegaEvolutionAnim(playerUnit.ChangeSprite);

			break;

			case BattleEvoSelection.Dynamax:

			        combatText.Append("极巨化了!");
			        await ShowCombatText();

			    playerUnit.Pokemon.DynamaxEvolution();
				await playerUnit.AnimStage.DynamaxEvolutionAnim();

			break;

			case BattleEvoSelection.Gigantamax:

			        combatText.Append("超极巨化了!");
			        await ShowCombatText();

				playerUnit.Pokemon.DynamaxEvolution();
			    await playerUnit.AnimStage.GigantamaxEvolutionAnim(playerUnit.ChangeSprite);

			break;
		}
		//玩家准备
		dialogBox.OpenChoosePanel();
	}
#endregion
#region 输出战斗文本
    /// <summary>
    /// 在文本框输出战斗文本
    /// </summary>
    /// <returns></returns>
	private async UniTask ShowCombatText()
	{
		await dialogBox.TypeDialog(combatText.ToString());
        combatText.Clear();
	}
#endregion
#region Queue<string>打印
	/// <summary>
	/// 文本队列
	/// </summary>
	/// <param name="queue">Queue<string> queue</param>
	/// <returns></returns>
	private async UniTask ShowQueueString(Queue<string> queue)
	{
        while(queue.Count > 0)
        {
            await dialogBox.TypeDialog(queue.Dequeue());
        }
	}
#endregion
#region 战斗选择栏按键
    //替换精灵按键
    public void SwitchButton()
    {
		choosePanel.SetDataForSwitch(playerTeam.Pokemons, false, currentPkmNum);
        dialogBox.CloseChoosePanel();
    }

	//背包按钮
	public void Bag()
	{
		UIManager.Instance.OpenBag();
	}

	public void UseItem(ItemBase _item, string detail, BattleAction action)
	{
		item = _item;
		combatText.Append(detail);
		RunTurns(action);
	}

    //逃跑按钮
    public async void EscapeBattle()
    {
		dialogBox.CloseChoosePanel();
		if(isTrainerBattle)
		{
			await dialogBox.TypeDialog("不能退缩!");
			await UniTask.Delay(300);
			dialogBox.OpenChoosePanel();
			return;
		}
		if(enemyUnit.Pokemon.Shiny)
		{
			UIManager.Instance.MessageTip.Tip("闪光");
			await dialogBox.TypeDialog("闪光!");
			await UniTask.Delay(300);
			dialogBox.OpenChoosePanel();
			return;
		}
		AbilityBase enemyAbility = enemyUnit.Pokemon.Ability;
		if(enemyAbility != null && enemyAbility.TriggerType == Ability_TriggerType.Enemies)
		{
			if((enemyAbility as EnemiesAbility).CheckEscape(playerUnit.Pokemon.Base.Type1, playerUnit.Pokemon.Base.Type2))
			{
                RunTurns(BattleAction.Run);
			}
			else
			{
				    combatText.Append("因为对手的");combatText.Append(enemyAbility.Name);
					combatText.Append("特性, 不能逃走!");
					await ShowCombatText();

			    await UniTask.Delay(300);
			    dialogBox.OpenChoosePanel();
				return;
			}
		}
		RunTurns(BattleAction.Run);
    }
#endregion
#region 技能选择按钮
    public async void RunSkill(int n)
	{
		Pokemon playerPokemon = playerUnit.Pokemon;
		Pokemon enemyPokemon = enemyUnit.Pokemon;
		if
		(
			(playerPokemon.Ability != null && playerPokemon.Ability.TriggerType == Ability_TriggerType.AllPlayer) ||
			(enemyPokemon.Ability != null && enemyPokemon.Ability.TriggerType == Ability_TriggerType.AllPlayer)
		)
		{
			if(!(playerPokemon.Ability as AllPlayerAbility).CanToRunSkill(playerPokemon.Skill[n]) || !(enemyPokemon.Ability as AllPlayerAbility).CanToRunSkill(playerPokemon.Skill[n]))
			{
				await dialogBox.TypeDialog("因为特性无法释放该技能!");
				await UniTask.Delay(200);
				dialogBox.OpenChoosePanel();
				return;
			}
		}
		//检查PP
		if(playerPokemon.Skill[n].CheckIfPPIsGreaterThanZero())
        {
			currentSkill = n;
            RunTurns(BattleAction.RunSkill);
        }
	}
#endregion
#region 学习技能
    private bool isFinishSelection() => buttonState != ButtonChoose.Selection;
    /// <summary>
    /// 打开升级学习技能面板
    /// </summary>
    /// <param name="pokemon"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
	private async UniTask LearnPanel(Pokemon pokemon, SkillBase skill)
	{
		state = BState.BUSY;
		await dialogBox.TypeDialog("选择一个技能替换, 或者放弃替换");
		choosePanel.ChooseSkillPanel.SetSkillDataForReplace(pokemon.Skill, skill, true);
		buttonState = ButtonChoose.Selection;
		await UniTask.WaitUntil(isFinishSelection);
		choosePanel.ChooseSkillPanel.OnClose();//*

		switch(buttonState)
		{
			case ButtonChoose.Learn:

			        combatText.Append(playerUnit.Pokemon.NickName);
				    combatText.Append("忘记了");
			        combatText.Append(playerUnit.Pokemon.Skill[learnNum].Base.SkillName);
				    combatText.Append("!");
			        await ShowCombatText();

			        combatText.Append("学会了");
				    combatText.Append(skill.SkillName);
				    combatText.Append("!");
			        await ShowCombatText();

			    playerUnit.Pokemon.Skill[learnNum] = new Skill(skill);

			break;

			case ButtonChoose.DontLearn:

		        await dialogBox.TypeDialog("没有学习新的技能");

			break;
		}
		state = BState.RunningTurn;
	}

	public void LearnSkill(bool yes, int n)
	{
		if(yes)//学
		{
			learnNum = n;
		    buttonState = ButtonChoose.Learn;
		}
		else//不学
		{
			buttonState = ButtonChoose.DontLearn;
		}
	}
#endregion
#region 公共选择Delegate
    /// <summary>
    /// 确认替换
    /// </summary>
    /// <param name="yes"></param>
    public void ConfirmSwitch(bool yes)
	{
		if(yes)
		{
			if(isFaintedSwitch)
			{
				#pragma warning disable 4014//不需要等待
				SwitchPokemon();
			}
			else
			{
				RunTurns(BattleAction.SwitchPokemon);
			}
		}
		else
		{
			if(isFaintedSwitch)
			{
				choosePanel.OnOpen();
			}
			else
			{
				dialogBox.OpenChoosePanel();
			}
		}
	}

	/// <summary>
	/// 确认继续战斗
	/// </summary>
	/// <param name="yes"></param>
	public void ContinueBattle(bool yes)
	{
        if(yes)
		{
			choosePanel.SetDataForSwitch(playerTeam.Pokemons, true, currentPkmNum);
		}
		else
		{
			if(isTrainerBattle)
			{
				choosePanel.OnOpen();
			}
			else
			{
				#pragma warning disable 4014//不需要等待
				ExitBattle();
			}
		}
	}
#endregion
}