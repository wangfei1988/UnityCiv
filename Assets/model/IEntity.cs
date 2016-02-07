using UnityEngine;
using System.Collections;

public abstract class IEntity : MonoBehaviour {

    public AudioSource audioSource { get; protected set; }

    protected virtual void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.maxDistance = 20;
        audioSource.minDistance = 2;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;

        TimeManager.instance.NeedNewOrders(this);
    }

    public abstract void Select();

    /// <summary>
    /// Returns whether the unit still needs orders for this turn or whether it knows what to do
    /// </summary>
    /// <returns></returns>
    public abstract bool NeedsOrders();
}
