using UnityEngine;
using System.Collections;

public class ProjectorBehaviour : MonoBehaviour
{
    public float mMoveSpeed = 1.0f;
    public Texture2D mProjectionTexture;

    private GameObject mProjBody;
    private GameObject mTargetBody;
    private bool mIsPickingProjBody = false;
    private bool mIsPickingTargetBody = false;
    private LineRenderer newLine;

    public Matrix4x4 GetView
    {
        get { return this.mProjBody.transform.worldToLocalMatrix; }
    }
    public Vector3 GetDir
    {
        get { return Vector3.Normalize(this.mTargetBody.transform.position - this.mProjBody.transform.position); }
    }

    // Use this for initialization
    void Start()
    {
        mProjBody = this.gameObject.transform.GetChild(0).gameObject;
        mTargetBody = this.gameObject.transform.GetChild(1).gameObject;
        newLine = mProjBody.GetComponent<LineRenderer>();
        newLine.SetColors(Random.ColorHSV(), Random.ColorHSV());
        newLine.SetWidth(0.01f, 0.01f);
        newLine.SetPosition(0, mProjBody.transform.position);
        newLine.SetPosition(1, mTargetBody.transform.position);
        for (int i = 0; i < mProjBody.transform.childCount; i++)
        {
            mProjBody.transform.GetChild(i).GetComponentInChildren<Renderer>().material.mainTexture = mProjectionTexture;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mProjBody.transform.LookAt(mTargetBody.transform.position);

        if (mIsPickingTargetBody)
        {
            // update target position
            Vector3 newPos = mTargetBody.transform.position;
            if (Input.GetKey(KeyCode.A))
                newPos.x -= mMoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                newPos.x += mMoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S))
                newPos.z -= mMoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.W))
                newPos.z += mMoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.F))
                newPos.y -= mMoveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.R))
                newPos.y += mMoveSpeed * Time.deltaTime;

            mTargetBody.transform.position = newPos;
            mTargetBody.GetComponentInChildren<Renderer>().material.color = Color.red;
            newLine = mProjBody.GetComponent<LineRenderer>();
            newLine.SetPosition(1, mTargetBody.transform.position);

        }
        else
        {
            mTargetBody.GetComponentInChildren<Renderer>().material.color = Color.white;
        }

        if (mIsPickingProjBody)
        {
            // update target position
            Vector3 newPos = mProjBody.transform.position;
            if (Input.GetKey(KeyCode.A))
                newPos.x -= mMoveSpeed * Time.deltaTime; ;
            if (Input.GetKey(KeyCode.D))
                newPos.x += mMoveSpeed * Time.deltaTime; ;
            if (Input.GetKey(KeyCode.S))
                newPos.z -= mMoveSpeed * Time.deltaTime; ;
            if (Input.GetKey(KeyCode.W))
                newPos.z += mMoveSpeed * Time.deltaTime; ;
            if (Input.GetKey(KeyCode.F))
                newPos.y -= mMoveSpeed * Time.deltaTime; ;
            if (Input.GetKey(KeyCode.R))
                newPos.y += mMoveSpeed * Time.deltaTime; ;

            mProjBody.transform.position = newPos;
            for (int i = 0; i< mProjBody.transform.childCount; i++)
            {
                mProjBody.transform.GetChild(i).GetComponentInChildren<Renderer>().material.color = Color.red;
            }
            newLine = mProjBody.GetComponent<LineRenderer>();
            newLine.SetPosition(0, mProjBody.transform.position);

        }
        else
        {
            for (int i = 0; i < mProjBody.transform.childCount; i++)
            {
                mProjBody.transform.GetChild(i).GetComponentInChildren<Renderer>().material.color = Color.white;
            }
        }

        // Picking code
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                if (mProjBody.Equals(hit.transform.gameObject))
                {
                    mIsPickingProjBody = true;
                    mIsPickingTargetBody = false;
                }
                else if (mTargetBody.Equals(hit.transform.gameObject))
                {
                    mIsPickingProjBody = false;
                    mIsPickingTargetBody = true;
                }
                else
                {
                    mIsPickingProjBody = mIsPickingTargetBody = false;
                }
            }
            else
            {
                mIsPickingProjBody = mIsPickingTargetBody = false;
            }
        }
    }
}
