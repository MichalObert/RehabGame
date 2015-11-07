using UnityEngine;
using System.Collections;
using Leap;
using System;

public class LeapMotionListener : Listener {

    //public delegate void ChangedEventHandler(object sender, EventArgs e);
    //public event ChangedEventHandler Changed;

   /* protected virtual void OnChanged(EventArgs e) {
        if(Changed != null)
            Changed(this, e);
    }*/
        public override void OnFrame(Controller controller) {
            Frame a = controller.Frame();
            Debug.Log("newFrame");
            foreach(Gesture g in a.Gestures()) {
                if(g.Type == Gesture.GestureType.TYPE_KEY_TAP) {
                    Debug.Log("working");
                    Vector3 positionOfTap = new Vector3(((KeyTapGesture)g).Position.x,((KeyTapGesture)g).Position.y,((KeyTapGesture)g).Position.z);
             //       PositionOfTapEventArgs positionOfTapEventArgs = new PositionOfTapEventArgs();
              //      positionOfTapEventArgs.PositionOfTap = positionOfTap;
                //    OnChanged(positionOfTapEventArgs);
                }
            }
        }


}
public class PositionOfTapEventArgs : EventArgs {
    public Vector3 PositionOfTap;
}