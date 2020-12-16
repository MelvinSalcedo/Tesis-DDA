using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour{
    public List<Action> actionsSlots = new List<Action>();

    public ItemAction consumableItem;

    StateManager states;

    public void Init(StateManager st) {
        states = st;
        UpdateActionsOneHanded();
    }

    public void UpdateActionsOneHanded() {
        EmtyAllSlots();
        Weapon w = states.invetoryManager.curWeapon;
        for (int i = 0; i < w.actions.Count; i++) {
            Action a = GetAction(w.actions[i].input);
            a.targetAnim = w.actions[i].targetAnim;
        }
    }

    public void UpdateActionsTwoHanded() {
        EmtyAllSlots();
        Weapon w = states.invetoryManager.curWeapon;
        for (int i = 0; i < w.two_handedActions.Count; i++) {
            Action a = GetAction(w.two_handedActions[i].input);
            a.targetAnim = w.two_handedActions[i].targetAnim;
        }
    }

    void EmtyAllSlots() {
        for (int i = 0; i < 4; i++) {
            Action a = GetAction((ActionInput)i);
            a.targetAnim = null;
            
        }
    }

    ActionManager() {
        for (int i = 0; i < 4; i++) {
            Action a = new Action();
            a.input = (ActionInput)i;
            actionsSlots.Add(a);
        }
    }


    public Action GetActionSlot(StateManager st) {
        ActionInput a_input = GetActionInput(st);
        return GetAction(a_input);
    }

    Action GetAction(ActionInput inp) {
       // return actionsSlots[(int)inp];

        for (int i = 0; i < actionsSlots.Count; i++) {
            if (actionsSlots[i].input == inp)
                return actionsSlots[i];
        }
        return null;
    }

    public ActionInput GetActionInput(StateManager st) {

        if (st.rb)
            return ActionInput.rb;
        if (st.rt)
            return ActionInput.rt;
        if (st.lb)
            return ActionInput.lb;
        if (st.lt)
            return ActionInput.lt;

        return ActionInput.rb;
    }

}

public enum ActionInput{
    rb,lb,rt,lt
}

[System.Serializable]
public class Action {
    public ActionInput input;
    public string targetAnim;
}

[System.Serializable]
public class ItemAction {
    public string targetAnim;
    public string item_id;

}
