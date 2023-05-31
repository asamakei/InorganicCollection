using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventExist : MonoBehaviour{
    public FlagDatas flag;
    public int EventFlagNumber = -1;
    public int TreasureFlagNumber = -1;
    public int TutorialFlagNumber = -1;
    public bool notVanish = false;
    void Start(){
        vanish();
    }
    public void changeEvent(int num) {
        if (EventFlagNumber >= 0) flag.Event[EventFlagNumber] = num;
        vanish();
    }
    public void changeTreasure(int num) {
        if (TreasureFlagNumber >= 0) flag.Treasure[TreasureFlagNumber] = num;
        vanish();
    }
    public void changeTutorial(int num) {
        if (TutorialFlagNumber >= 0) flag.Tutorial[TutorialFlagNumber] = num;
        vanish();
    }
    public int getEvent() {
        if (EventFlagNumber >= 0)return flag.Event[EventFlagNumber];
        return -1;
    }
    public int getTreasure() {
        if (TreasureFlagNumber >= 0)return flag.Treasure[TreasureFlagNumber];
        return -1;
    }
    public int getTutorial() {
        if (TutorialFlagNumber >= 0) return flag.Tutorial[TutorialFlagNumber];
        return -1;
    }
    void Update() {
        vanish();
    }
    private void vanish() {
        if (notVanish) return;
        if (EventFlagNumber >= 0 && flag.Event[EventFlagNumber] == -1) Destroy(this.gameObject);
        if (TreasureFlagNumber >= 0 && flag.Treasure[TreasureFlagNumber] == -1) Destroy(this.gameObject);
        if (TutorialFlagNumber >= 0 && flag.Tutorial[TutorialFlagNumber] == -1) Destroy(this.gameObject);
    }
}
