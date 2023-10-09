using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System;
using UnityEngine.Networking;
using System.Net;
using System.Text;
using System.IO;
using TMPro;

public class VPSManager : MonoBehaviour
{

    [SerializeField]private AREarthManager earthManager;

    [Serializable]
    public struct EarthPosition{
        public double Latitude;
        public double Longitude;
        public double Altitude;
    }
    
    [Serializable]
    public struct GeospatialObject{
        public GameObject ObjectPrefab;
        public EarthPosition EarthPosition;
    }

    [SerializeField] private ARAnchorManager ARAnchorManager;
    [SerializeField] public List<GeospatialObject> geospatialObjects = new List<GeospatialObject>();

    public TextMeshPro resultado;

    // Start is called before the first frame update
    void Start()
    {
        verifyGeospatialSupport();
    }

    private void verifyGeospatialSupport(){
        var result = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch(result){
            case FeatureSupported.Supported:
                Debug.Log("Ready to use VPS");
                getEntities();
                placeObjects();
                break;

            case FeatureSupported.Unknown:
                Debug.Log("Unknown...");
                Invoke("verifyGeospatialSupport", 5.0f);
                break;

            case FeatureSupported.Unsupported:
                Debug.Log("VPS Unsupported");
                break;

        }
    }

    private void placeObjects(){
        if (earthManager.EarthTrackingState == TrackingState.Tracking){

            
            
            foreach( var obj in geospatialObjects){

                var earthPosition = obj.EarthPosition;
                var modelAnchor = ARAnchorManagerExtensions.AddAnchor(ARAnchorManager, earthPosition.Latitude, earthPosition.Longitude, earthPosition.Altitude, Quaternion.identity);
                // obj.ObjectPrefab.active = true;
                Instantiate(obj.ObjectPrefab, modelAnchor.transform);

            }
        } else if (earthManager.EarthTrackingState == TrackingState.None){
            Invoke("placeObjects", 0.5f);
            // resultado.text = "Parado";
        }
    }

    private IEnumerator getEntities() {
        UnityWebRequest www = UnityWebRequest.Get("192.168.100.12:8070/getEntityByEcosistema/63c992b466067a7760322a76");
        yield return www.Send();
 
        if(www.isNetworkError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            resultado.text = www.downloadHandler.text;

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
}
