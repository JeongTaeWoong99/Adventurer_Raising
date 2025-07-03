using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;

// 기본정보(생성 및 저장될 때, 필요한 정보 직렬화)
public class DefaultData
{
	public string email;        // 이메일
	public string creationDate; // 요청했을 때 날짜
	public string nickname;
	public string serialNumber;
	public string currentLevel;
	public string currentHp;
	public string currentExp;
	public string currentGold;
	public string savedScene;		
}

public class RealTimeManager
{
	public DefaultData myDefaultData; // 저장 정보
	
	public void Init()
	{
		myDefaultData = new DefaultData();
	}
}