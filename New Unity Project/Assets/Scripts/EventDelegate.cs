using UnityEngine;
using System.Collections;
using Leap;

public class EventDelegate : MonoBehaviour {

    delegate void LeapEventDelegate(string EventName);

    /** This method check the event in listener class
       *The activated event's name can be got through this method*/
    public void LeapEventNotification(string EventName) {
        if(this) {
            switch(EventName) {
                case "onInit":

                    break;
                case "onConnect":

                    break;
                case "onFrame":

                    break;
            }
        } else {

        }
    }
}//