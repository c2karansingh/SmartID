using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.Collections;
using System;
using TMPro;
using Proyecto26;
public class BadgeTrackerBehaviour : MonoBehaviour,ITrackableEventHandler
{
    public RawImage rawImage;
    static string imgLink;
    int id,prevId = 0;
    public GameObject loadingGif,faceLoadingGif,mask;
    TrackableBehaviour badgeTrackableBehaviour;
    VuMarkManager VuMark;
    public TMP_Text userName,designation;

    // Start is called before the first frame update
    void Start()
    {
        badgeTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (badgeTrackableBehaviour != null)
        {
            badgeTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
        VuMark = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED || newStatus == TrackableBehaviour.Status.TRACKED || newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            OnTrackerFound();
        }
        else if (previousStatus == TrackableBehaviour.Status.TRACKED && newStatus != TrackableBehaviour.Status.TRACKED)
        {
            OnTrackerLost();
        }
    }
    private void OnTrackerFound()
    {
        foreach (var items in VuMark.GetActiveBehaviours())
        {
            id = Convert.ToInt32(items.VuMarkTarget.InstanceId.NumericValue);

            Debug.Log("id : " + id);
            FetchData();
        }
    }
    private void OnTrackerLost()
    {
        prevId = id;
    }

    void FetchData()
    {
        Debug.Log("in fetch");
        if(prevId!=id)
        {
            Debug.Log("in if");
            userName.text = "";
            designation.text = "";
            mask.SetActive(false);
            loadingGif.SetActive(true);
        }
        RestClient.Get<UserData>("https://royaldata-2590.firebaseio.com/users/" + id + ".json").Then(GetValue =>
        {
            loadingGif.SetActive(false);
            Debug.Log("Name : " + GetValue.userName);
            imgLink = GetValue.imageLink;
            if(prevId!=id)
            {
                faceLoadingGif.SetActive(true);
            }
            StartCoroutine("GetImage");

            userName.text = GetValue.userName;
            designation.text = "Designation : " + GetValue.userDesignation;
            //course.text = "Teaches : " + GetValue.user_id;

        });
    }
    public IEnumerator GetImage()
    {
        WWW www = new WWW(imgLink);
        yield return www;
        faceLoadingGif.SetActive(false);
        mask.SetActive(true);
        rawImage.texture = www.texture;
    }
}
