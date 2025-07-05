using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using LJ2;
using TMPro;
using KYG_skyPower;

namespace JYL
{
    public class MainPresenter : BaseUI
    {
        private Image charImg1;
        private Image charImg2;
        private Image charImg3;
        private event Action onEnterMain;
        private CharacterSaveLoader characterLoader;
        private TMP_Text unitText;
        void Start()
        {
            characterLoader = GetComponent<CharacterSaveLoader>();
            characterLoader.GetCharPrefab();
            unitText = GetUI<TMP_Text>("UnitText");
            charImg1 = GetUI<Image>("CharImage1");
            charImg2 = GetUI<Image>("CharImage2");
            charImg3 = GetUI<Image>("CharImage3");
            SetPartyImage();
            GetEvent("ShopBtn").Click += OpenShop;
            GetEvent("PartySetBtn").Click += OpenPartySetting;
            GetEvent("PlayBtn").Click += OpenGameMode;
            unitText.text = $"{Manager.Game.CurrentSave.gold} UNIT";
            // GetEvent("InfoBtn").Click += OpenGameInfo;
        }
        private void LateUpdate()
        {
            CheckPopUp();
        }
        private void OpenShop(PointerEventData eventData)
        {
            // TODO : 상점 구현
            // GameSceneManager.Instance.SceneChange("Shop");
            SceneManager.LoadSceneAsync("cStoreScene_JYL");
        }
        private void OpenPartySetting(PointerEventData eventData)
        {
            UIManager.Instance.ShowPopUp<PartySetPopUp>();
        }
        private void OpenGameMode(PointerEventData eventData)
        {
            UIManager.Instance.ShowPopUp<GameModePopUp>();
        }
        //private void OpenGameInfo(PointerEventData eventData)
        //{
        //    // TODO : 후순위 구현 예정
        //}
        private void SetPartyImage()
        {
            charImg1.sprite = characterLoader.mainController.image;
            charImg2.sprite = characterLoader.sub1Controller.image;
            charImg3.sprite = characterLoader.sub2Controller.image;
        }
        private void CheckPopUp()
        {
            if (PopUpUI.IsPopUpActive && onEnterMain == null)
            {
                onEnterMain += characterLoader.GetCharPrefab;
                onEnterMain += SetPartyImage;
            }
            else if (!PopUpUI.IsPopUpActive)
            {
                onEnterMain?.Invoke();
                if (onEnterMain != null)
                {
                    onEnterMain -= characterLoader.GetCharPrefab;
                    onEnterMain -= SetPartyImage;
                }
            }
        }
        private void UpdateUnitText()
        {
            unitText.text = $"{Manager.Game.CurrentSave.gold} UNIT";
        }
    }
}

