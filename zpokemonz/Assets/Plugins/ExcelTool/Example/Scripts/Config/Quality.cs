namespace ExcelData
{
	/// <summary>
	/// 品质表
	/// <summary>
	[System.Serializable]
	public  class Quality
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
	/// 颜色
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("颜色")]
	public string color;
	/// <summary>
	/// 说明
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("说明")]
	public string description;


		#endregion

	}
}
