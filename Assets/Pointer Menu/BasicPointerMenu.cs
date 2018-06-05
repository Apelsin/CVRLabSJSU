﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(DestinationMarkerEventReceiver))]
[RequireComponent(typeof(VRTK_InteractableObject))]
public class BasicPointerMenu : PointerMenuBase
{
    private const int UI_LAYER_MASK = 32;
    public GameObject MenuPrefab;
    public LayerMask MenuLayerIgnoreMask = ~UI_LAYER_MASK;
    public MenuButtons Template;

    public ButtonInfo[] Buttons;

    private struct PointerMenuData
    {
        public PointerContextMenu Menu;
        public LayerMask OriginalIgnoreMask;
    }

    private Dictionary<VRTK_Pointer, PointerMenuData> PointerMenus = new Dictionary<VRTK_Pointer, PointerMenuData>();
    private Dictionary<VRTK_Pointer, Vector3> Destinations = new Dictionary<VRTK_Pointer, Vector3>();

    protected override void OnUse(VRTK_Pointer pointer)
    {
        var button_behavior_manager =
            GameObject.FindGameObjectWithTag("Behavior Manager")?
            .GetComponentInChildren<ButtonBehaviorManager>();
        if (!button_behavior_manager)
            Debug.LogWarning("Could not find button behavior manager.");
        if (!PointerMenus.ContainsKey(pointer))
        {
            var menu_object = Instantiate(MenuPrefab);
            var pointer_context_menu = menu_object.GetComponent<PointerContextMenu>();
            var pointer_layers_to_ignore = pointer.pointerRenderer.layersToIgnore;
            var target_position = Destinations[pointer];
            pointer_context_menu.TargetPosition = target_position;
            pointer_context_menu.MainCameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
            PointerMenus[pointer] = new PointerMenuData()
            {
                Menu = pointer_context_menu,
                OriginalIgnoreMask = pointer_layers_to_ignore
            };
            pointer.pointerRenderer.layersToIgnore = MenuLayerIgnoreMask;
            var map = button_behavior_manager?.GetButtonBehaviorsMap();
            pointer_context_menu.SetMenuButtons(Buttons, map);
        }
    }

    protected override void OnDestinationMarkerEnter(VRTK_Pointer pointer, Vector3 destination_position)
    {
        Destinations[pointer] = destination_position;
    }

    public void Start()
    {
        if (Template)
            Buttons = Template.Buttons.ToArray();
        else
            Debug.LogWarning("Menu has no template.");
        if (Buttons.Length == 0)
            Debug.LogWarning("Menu has no buttons.");
    }

    private void Update()
    {
        foreach (var kvp in PointerMenus.ToArray())
        {
            var pointer = kvp.Key;
            var menu_data = kvp.Value;
            // If the controller button is released, remove the menu
            if (!pointer.controller.IsButtonPressed(pointer.activationButton))
            {
                PointerMenus.Remove(pointer);
                menu_data.Menu.RequestDestroy();
                pointer.pointerRenderer.layersToIgnore = menu_data.OriginalIgnoreMask;
            }
        }
    }
}