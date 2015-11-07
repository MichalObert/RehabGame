using UnityEngine;
using System.Collections;
using Leap;

public class FrameListener : Listener {
    //public Frame currentFrame;

    public delegate void LeapEventDelegate(object sender);
    public LeapEventDelegate eventDelegate;
 
        //create a constructor with interface argument
    public FrameListener(LeapEventDelegate delegateObject)
    {
        //create a object of interface
        this.eventDelegate = delegateObject; 
    }
    public FrameListener() {
    }

    //private static readonly FrameListener instance = new FrameListener();
   



    //public static FrameListener Instance
    //{
    //    get 
    //    {
    //        return instance; 
    //    }
    //}
    
    public override void OnFrame(Controller controller) {
       // this.eventDelegate.LeapEventNotification(currentFrame);
        //this.eventDelegate.LeapEventNotification();
        Debug.Log("Frame");
        if(eventDelegate != null) {
            eventDelegate(this);
        }
    }
}
