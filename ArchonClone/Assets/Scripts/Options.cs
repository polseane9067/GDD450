﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Options : MonoBehaviour {

    public EventSystem es; 

    public GameObject BackToMenuButton;
    public GameObject BackToOptionsButton;

    public GameObject ControlsPanel;
    public GameObject SettingsPanel;
    public GameObject OptionsPanel;
    public GameObject CreditsPanel;

    public GameObject ControllerPanel;
    public GameObject LocalPanel;
    public GameObject SingleLANPanel;

    public ToggleGroup ViewToggle;

    public Toggle TopDownMark;
    public Toggle ThirdPersonMark;

    public GameObject firstButtonControlsPage;
    public GameObject firstButtonSettingsPage;
    public GameObject firstButtonOptionsPage;

    void Awake()
    {
        ViewToggle.RegisterToggle(TopDownMark);
        ViewToggle.RegisterToggle(ThirdPersonMark);
    }

    void Start()
    {
        if (BattleStats.player1Pref == "third" && BattleStats.player2Pref == "third")
        {
            ThirdPersonMark.isOn = true;
        }
        else if (BattleStats.player1Pref == "top" && BattleStats.player2Pref == "top")
        {
            TopDownMark.isOn = true;
        }
    }

    public void backToOptions()
    {
        es.SetSelectedGameObject(firstButtonOptionsPage);
        BackToMenuButton.SetActive(true);
        BackToOptionsButton.SetActive(false);

        OptionsPanel.SetActive(true);
        ControlsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }

    public void toControls()
    {
        es.SetSelectedGameObject(firstButtonControlsPage);
        BackToMenuButton.SetActive(false);
        BackToOptionsButton.SetActive(true);

        OptionsPanel.SetActive(false);
        ControlsPanel.SetActive(true);
        SettingsPanel.SetActive(false);

        LocalPanel.SetActive(true);
        ControllerPanel.SetActive(false);
        SingleLANPanel.SetActive(false);
    }

    public void toSettings()
    {
        es.SetSelectedGameObject(firstButtonSettingsPage);
        BackToMenuButton.SetActive(false);
        BackToOptionsButton.SetActive(true);

        OptionsPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    public void nextPage()
    {
        if (ControllerPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(false);
            LocalPanel.SetActive(true);
            SingleLANPanel.SetActive(false);
        }
        else if (LocalPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(false);
            LocalPanel.SetActive(false);
            SingleLANPanel.SetActive(true);
        }
        else if (SingleLANPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(true);
            LocalPanel.SetActive(false);
            SingleLANPanel.SetActive(false);
        }
    }

    public void previousPage()
    {
        if (ControllerPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(false);
            LocalPanel.SetActive(false);
            SingleLANPanel.SetActive(true);
        }
        else if (LocalPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(true);
            LocalPanel.SetActive(false);
            SingleLANPanel.SetActive(false);
        }
        else if (SingleLANPanel.activeInHierarchy == true)
        {
            ControllerPanel.SetActive(false);
            LocalPanel.SetActive(true);
            SingleLANPanel.SetActive(false);
        }
    }

    public void toggleView()
    {
        if (TopDownMark.isOn == true)
        {
            BattleStats.player1Pref = "top";
            BattleStats.player2Pref = "top";
        }
        else if (ThirdPersonMark.isOn == true)
        {
            BattleStats.player1Pref = "third";
            BattleStats.player2Pref = "third";
        }
    }
}
