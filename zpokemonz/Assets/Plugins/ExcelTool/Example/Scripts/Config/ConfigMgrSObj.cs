namespace ExcelData
{
	/// <summary>
	/// 
	/// <summary>
	[System.Serializable]
	public  class ConfigMgrSObj : Sirenix.OdinInspector.SerializedScriptableObject
	{
		#region ����

	/// <summary>
	/// 语言表
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("语言表")]
	public Language[]  languages;
	/// <summary>
	/// 属性表
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("属性表")]
	public Property[]  propertys;
	/// <summary>
	/// 品质表
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("品质表")]
	public Quality[]  qualitys;


		#endregion

	}
}
