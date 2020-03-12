using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    // Start is called before the first frame update
    public void TriggerPopup()
    {
        gameObject.GetComponent<Animator>().SetTrigger("popup");
        StartCoroutine(DestroyObject());
    }

    IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
