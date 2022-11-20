namespace EventManager {
    using UnityEngine;
    [DisallowMultipleComponent]
	public class KuchenSubscriberGameObject : MonoBehaviour
	{
		public Subscriber Subscriber { get; private set; }
		
		public KuchenSubscriberGameObject()
		{
			Subscriber = new Subscriber();
		}
		
		public void OnDestroy()
		{
			Subscriber.Dispose();
		}
	}
}