using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public Renderer MeshRender;

    public Color NoCollisionColor = new Color(164, 164, 164);
    public Color CollisionColor = new Color(255, 0, 0);

    public bool IsColliding = false;

    public bool Broken = false;

    private void Awake()
    {
        NoCollisionColor = MeshRender.material.color;
    }

    public bool IsResponding()
    {
        if(Broken == true)
        {
            return false;
        }
        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(Broken == true)
        {
            IsColliding = false;
            MeshRender.material.color = NoCollisionColor;
            return;
        }

        if(collision.transform.tag != this.transform.tag && collision.transform.tag != "RobotCollidable")
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }

        if(collision.transform.tag == "RobotCollidable")
        {
            IsColliding = true;
            MeshRender.material.color = CollisionColor;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(Broken == true)
        {
            IsColliding = false;
            MeshRender.material.color = NoCollisionColor;
            return;
        }

        if(collision.transform.tag != this.transform.tag && collision.transform.tag != "RobotCollidable")
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }

        if(collision.transform.tag == "RobotCollidable")
        {
            IsColliding = true;
            MeshRender.material.color = CollisionColor;
        }        
    }

    private void OnCollisionExit(Collision collision)
    {
        if(Broken == true)
        {
            IsColliding = false;
            MeshRender.material.color = NoCollisionColor;
            return;
        }

        if (collision.transform.tag == "RobotCollidable")
        {
            IsColliding = false;
            MeshRender.material.color = NoCollisionColor;
        }
    }

}
