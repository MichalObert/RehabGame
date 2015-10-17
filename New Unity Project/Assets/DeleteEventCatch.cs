

using UnityEngine;
using System.Collections;
using System;
public class DeleteEventCatch : MonoBehaviour {
    private DeleteEventSend Send;  
    public DeleteEventCatch(DeleteEventSend send) 
      {
         Send = send;
         // Add "ListChanged" to the Changed event on "List":
         Send.eventHandler += new EventHandler(EventInvoked);
      }

      // This will be called whenever the list changes:
      private void EventInvoked(object sender, EventArgs e) 
      {
        Debug.Log("This is called when the event fires.");
      }

      public void Detach() 
      {
         // Detach the event and delete the list:
         Send.eventHandler -= new EventHandler(EventInvoked);
         Send = null;
      }
      public void Start() {
          Debug.Log("On start called");
          // Create a new list:
          DeleteEventSend send = new DeleteEventSend();

          // Create a class that listens to the list's change event:
          DeleteEventCatch listener = new DeleteEventCatch(send);

          // Add and remove items from the list:
          send.invokeTheEvent(4);
          listener.Detach();
      }
   }

class Test : MonoBehaviour
   {
      // Test the ListWithChangedEvent class:
      public void OnStart() 
      {
          Debug.Log("On start called");
      // Create a new list:
     DeleteEventSend send = new DeleteEventSend();

      // Create a class that listens to the list's change event:
      DeleteEventCatch listener = new DeleteEventCatch(send);

      // Add and remove items from the list:
      send.invokeTheEvent(4);
      listener.Detach();
      }
   }
