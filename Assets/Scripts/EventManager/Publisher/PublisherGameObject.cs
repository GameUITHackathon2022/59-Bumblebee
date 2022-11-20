namespace EventManager {
    using UnityEngine;
    [DisallowMultipleComponent]
	public class KuchenPublisherGameObject : MonoBehaviour
	{
		public void Publish(string topic)
		{
			Publisher.Publish(topic);
		}
	}
}