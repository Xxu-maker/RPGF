namespace ExcelData
{
	/// <summary>
	/// 怪物表
	/// <summary>
	[System.Serializable]
	public  class Monster
	{
		#region ����

	/// <summary>
	/// 编号
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("编号")]
	public int id;
	/// <summary>
	/// 名称
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("名称")]
	public string name;
	/// <summary>
	/// 类型：1-普通小怪，2-精英小怪，3-头目，4-普通boss，
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("类型：1-普通小怪，2-精英小怪，3-头目，4-普通boss，")]
	public EnumMonsterLevel monsterLevel_id;
	/// <summary>
	/// 说明(&代表具体数值取决于对应属性)
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("说明(&代表具体数值取决于对应属性)")]
	public string explain;
	/// <summary>
	/// 图片
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("图片")]
	public string sprite_path;


		#endregion

	}
}
