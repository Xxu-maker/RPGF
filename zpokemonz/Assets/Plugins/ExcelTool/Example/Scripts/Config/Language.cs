namespace ExcelData
{
	/// <summary>
	/// 语言表
	/// <summary>
	[System.Serializable]
	public  class Language
	{
		#region ����

	/// <summary>
	/// 索引
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("索引")]
	public string key;
	/// <summary>
	/// 简体中文
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("简体中文")]
	public string CN;
	/// <summary>
	/// 繁体中文
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("繁体中文")]
	public string HK;
	/// <summary>
	/// 英文
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("英文")]
	public string EN;
	/// <summary>
	/// 日文
	/// <summary>
	[UnityEngine.SerializeField, UnityEngine.Header("日文")]
	public string JP;


		#endregion

	}
}
