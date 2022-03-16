using UnityEngine;

public class CameraPlayer : MonoBehaviour
{

    private Transform lookAt;
    public Vector3 startOffset;

    //private Vector3 moveVector;
    //private Vector3 animationOffSet = new Vector3(0, 5, 5);

    // Use this for initialization
    void Start()
    {
        lookAt = GameObject.FindGameObjectWithTag("Player").transform;
        //startOffset = transform.position - lookAt.position;
        //startOffset = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //moveVector = lookAt.position + startOffset;
        transform.position = new Vector3(lookAt.position.x, transform.position.y, lookAt.transform.position.z - 8f); //+ startOffset;

        transform.rotation = Quaternion.Euler(transform.rotation.x, lookAt.eulerAngles.y, 0);
        //X
        //moveVector.x = 0;
        //Y
        //moveVector.y = Mathf.Clamp(moveVector.y, 3, 5);

        //if (transition > 1)
        //{
        //    transform.position = moveVector;
        //}
        //else if(transition < 1)
        //{
        //Animation to start
        //transform.position = Vector3.Lerp(moveVector + animationOffSet, moveVector, transition);
        //transition += Time.deltaTime * 1 / animationDuration;
        //transform.LookAt(lookAt.position + Vector3.up);
        //}
    }
}
