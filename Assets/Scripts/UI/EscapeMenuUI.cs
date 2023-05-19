using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuUI : MonoBehaviour
{

    private bool MenuIsOpen
    {
        get => _menuIsOpen;
        set
        {
            _menuIsOpen = value;
            _graphicsRoot.gameObject.SetActive(_menuIsOpen);
        }
    }
    
    [SerializeField] private RectTransform _graphicsRoot;
    private bool _menuIsOpen;

    private void Start()
    {
        _menuIsOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuIsOpen = !MenuIsOpen;
        }
    }

    public void ToggleMenu(bool open)
    {
        MenuIsOpen = open;
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
