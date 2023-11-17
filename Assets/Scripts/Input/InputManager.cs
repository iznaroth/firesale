using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EActionMap
{
    IN_GAME,
    MINIGAME,
    UI
}

public enum EInGameAction
{
    MOVE,
    PICK_UP,
    ABILITY,

}

public enum EMinigameAction
{
    SELECT,

}

public enum EUIAction
{
    SELECT,

}

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    #region StaticInterface
    static InputManager instance;

    public static InputManager GetInstance()
    {
        if (instance)
        {
            return instance;
        }

        Debug.LogWarning("Warning: No input manager instance found. Creating default one");

        GameObject newManagerGO = new GameObject("DefaultInputManager");
        return newManagerGO.AddComponent<InputManager>();
    }

    static Dictionary<EActionMap, string> actionMapNames = new Dictionary<EActionMap, string>
    {
        { EActionMap.IN_GAME, "InGame" },
        { EActionMap.MINIGAME, "Minigame" },
        { EActionMap.UI, "UI" },
    };

	static Dictionary<EInGameAction, string> inGameActionNames = new Dictionary<EInGameAction, string>
	{
        { EInGameAction.MOVE, "Move" },
        { EInGameAction.PICK_UP, "Pick Up" },
        { EInGameAction.ABILITY, "Ability" },
    };

    static Dictionary<EMinigameAction, string> minigameActionNames = new Dictionary<EMinigameAction, string>
    {
        { EMinigameAction.SELECT, "Select" },
    };

    static Dictionary<EUIAction, string> uiActionNames = new Dictionary<EUIAction, string>
    {
        { EUIAction.SELECT, "Select" },
    };


    // Switch to new action map while keeping track of the one we were just using
    public static void PushActionMap(EActionMap newMap)
    {
        GetInstance().PushActionMap_Internal(newMap);
    }

    // Leave current action map and go back to previous one
    public static void PopActionMap()
    {
        GetInstance().PopActionMap_Internal();
    }

    // Set new action map without saving the current one, and optionally erasing the saved action map stack
    public static void OverrideActionMap(EActionMap newMap, bool clearStack = false)
    {
        GetInstance().OverrideActionMap_Internal(newMap, clearStack);
    }

    public static InputAction GetInputAction(EInGameAction action)
	{
        InputActionAsset controls = GetInstance().playerInput.actions;
        InputActionMap map = controls.FindActionMap(actionMapNames[EActionMap.IN_GAME]);
        return map.FindAction(inGameActionNames[action]);
	}

    public static InputAction GetInputAction(EMinigameAction action)
	{
        InputActionAsset controls = GetInstance().playerInput.actions;
        InputActionMap map = controls.FindActionMap(actionMapNames[EActionMap.MINIGAME]);
        return map.FindAction(minigameActionNames[action]);
    }

    public static InputAction GetInputAction(EUIAction action)
    {
        InputActionAsset controls = GetInstance().playerInput.actions;
        InputActionMap map = controls.FindActionMap(actionMapNames[EActionMap.UI]);
        return map.FindAction(uiActionNames[action]);
    }

    #endregion // StaticInterface


    public EActionMap defaultMap = EActionMap.IN_GAME;

    PlayerInput playerInput;
    Stack<EActionMap> actionMapStack = new Stack<EActionMap>(); // the stack saves the previous action maps used (does NOT include the current map)
    EActionMap currentMap;



    void Awake()
    {
        // make sure we're the only instance
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Warning: Duplicate input managers detected, destroying " + name);
            Destroy(this);
            return;
        }

        instance = this;

        // make sure we have a player input to use
        if (!(playerInput = GetComponent<PlayerInput>()))
        {
            Debug.LogWarning("Warning: No PlayerInput component found on " + name + ", creating one");
            playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.actions = FindAnyObjectByType<InputActionAsset>();
        }
    }

    private void Start()
    {
        // set up the default action map
        playerInput.SwitchCurrentActionMap(actionMapNames[defaultMap]);
        currentMap = defaultMap;
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    void SetActionMap(EActionMap newMap)
    {
        if (currentMap != newMap)
        {
            playerInput.SwitchCurrentActionMap(actionMapNames[newMap]);
        }

        currentMap = newMap;
    }

    void PushActionMap_Internal(EActionMap newMap)
    {
        actionMapStack.Push(currentMap);
        SetActionMap(newMap);
    }

    void PopActionMap_Internal()
    {
        // if the stack is empty, go back to the default map
        if (actionMapStack.Count < 1)
        {
            SetActionMap(defaultMap);
            return;
        }

        // otherwise, use the map on the stop of the stack
        EActionMap newMap = actionMapStack.Pop();
        SetActionMap(newMap);
    }

    void OverrideActionMap_Internal(EActionMap newMap, bool clearStack)
    {
        SetActionMap(newMap);

        if (clearStack)
        {
            actionMapStack.Clear();
        }
    }
}
