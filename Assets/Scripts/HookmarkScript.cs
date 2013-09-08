using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class HookmarkScript : MonoBehaviour {
	
	private List<LineRenderer> lines = new List<LineRenderer>();
	private LineRenderer prevLine;
	private LeapUnityBridge targetScript;
	private GameObject finger;
	public bool m_Use_mouse;

	// Initialization
	void Start () {
		//Add an event of PointableUpdated.
		LeapInput.PointableUpdated += new LeapInput.PointableUpdatedHandler(OnPointableUpdated);
		
		targetScript = GameObject.Find ("LeapController").GetComponent<LeapUnityBridge>();
		
		if (m_Use_mouse) {
			UseMouseFinger();
		}
	}
	
	void Update () {
		if (m_Use_mouse)
		{
			UpdateFingerPositionWithMouse();
		}
		
		if (m_Use_mouse && prevLine != null)
        {
            Vector3 pos = finger.transform.position;
			prevLine.SetPosition(1, pos);
        }
	}
	
	/**
	 * Use finger and controling from mouse if m_Use_mouse is true.
	 */
	void UseMouseFinger()
	{
		LeapUnityHandController controller = GameObject.Find ("Leap Hands").GetComponent<LeapUnityHandController>();
		finger = controller.m_fingers[0];
		controller.SetVisible(finger, true);
		controller.SetCollidable(finger, true);
	}
	
	/**
	 * Update finger position from mouse position.
	 */
	void UpdateFingerPositionWithMouse()
	{
		Vector3 screen_point = Input.mousePosition;
		screen_point.z = 7.8f;
		
		Vector3 world_position = Camera.main.ScreenToWorldPoint(screen_point);
		finger.transform.position = world_position;
	}
	
	void OnPointableUpdated(Pointable p)
	{
        if (prevLine != null && p.IsValid)
        {
            //Vector3 dir = p.Direction.ToUnity();
            Vector3 pos = p.TipPosition.ToUnityTranslated();
            pos.z -= targetScript.m_LeapOffset.z;

            //transform.localPosition = pos;
            //transform.localRotation = Quaternion.FromToRotation(Vector3.forward, dir);
			prevLine.SetPosition(1, pos);
        }
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Touchable") {
			
			//Current collision object position.
			Vector3 pos = collision.gameObject.transform.position;
			
			if (prevLine) {
				prevLine.SetPosition(1, pos);
			}
			
			//Changed a material of touched object.
			Material material = Resources.Load("Materials/HookmarkPointMaterialActive") as Material;
			collision.gameObject.renderer.material = material;
			
			GameObject obj = new GameObject("lineObj");
			obj.AddComponent("LineRenderer");
			
			prevLine = obj.GetComponent<LineRenderer>();
			Material mat = new Material(Shader.Find ("Diffuse"));
			mat.color = Color.red;
			prevLine.materials = new Material[] {
				mat
			};
			prevLine.SetVertexCount(2);
			prevLine.SetWidth(0.4f, 0.4f);
			prevLine.SetPosition(0, pos);
			lines.Add(prevLine);
		}
	}
}
