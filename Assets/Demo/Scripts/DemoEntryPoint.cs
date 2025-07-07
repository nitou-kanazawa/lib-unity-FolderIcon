using UnityEngine;
using System.Collections;

public class DemoEntryPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f); // Wait for a short time to ensure everything is initialized

        Debug.Log("Demo Entry Point: Start");

        int x = Random.Range(0, 100);

        yield return new WaitForSeconds(1f); // Wait for a short time to ensure everything is initialized
        Debug.Log("Demo Entry Point: dddd");


        yield return new WaitForSeconds(2f); // Wait for a short time to ensure everything is initialized
        Debug.Log("Demo Entry Point: end");
    }

}
