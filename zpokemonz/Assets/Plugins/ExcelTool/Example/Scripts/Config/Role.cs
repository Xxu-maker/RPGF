namespace ExcelData
{
	/// <summary>
	/// 职业
	/// <summary>
	[System.Serializable]
	public  class Role
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
	/// 说明
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("说明")]
	public string describe;
	/// <summary>
	/// 图标
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("图标")]
	public UnityEngine.Sprite icon;


		#endregion

	}
}
