using UnityEngine;
using System.Collections;
using System;

public class DeleteEventSend : MonoBehaviour {
      // An event that clients can use to be notified whenever the
      // elements of the list change:
      public event EventHandler eventHandler;

      // Invoke the Changed event; called whenever list changes:
      protected virtual void invokeEvent(EventArgs e) 
      {
         if (eventHandler != null)
            eventHandler(this,e);
      }

      // Override some of the methods that can change the list;
      // invoke event after each:
      public int invokeTheEvent(int value) 
      {
         invokeEvent(EventArgs.Empty);
         return value;
      }

   }


