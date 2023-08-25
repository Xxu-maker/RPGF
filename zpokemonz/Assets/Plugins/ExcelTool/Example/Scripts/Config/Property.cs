namespace ExcelData
{
	/// <summary>
	/// 属性表
	/// <summary>
	[System.Serializable]
	public  class Property
	{
		#region ����

	/// <summary>
	/// 编号
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("编号")]
	public int id;
	/// <summary>
	/// 类型
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("类型")]
	public string type;
	/// <summary>
	/// 名称
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("名称")]
	public string name;
	/// <summary>
	/// 说明
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("说明")]
	public string description;
	/// <summary>
	/// 图片
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("图片")]
	public string sprite_path;


		#endregion

	}
}
