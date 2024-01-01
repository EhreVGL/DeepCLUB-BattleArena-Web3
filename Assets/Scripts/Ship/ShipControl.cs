using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    public float denizEtkisi = 0.5f;
    [SerializeField]
    GameObject ocean;

    public float hareketHizi = 5.0f;
    public float donusHizi = 2.0f;

    private Rigidbody shipRb;

    void Start()
    {
        shipRb = GetComponent<Rigidbody>();
        shipRb.freezeRotation = true; // Geminin fizik tabanlý dönüþleri için bu ayarý kullanabilirsiniz
    }
    private void Update()
    {
        Vector3 dalgaYonu = DenizDalgasiYonunuAl();
        //transform.Translate(dalgaYonu * denizEtkisi * Time.deltaTime);
    }
    private Vector3 DenizDalgasiYonunuAl()
    {
        if (ocean != null)
        {
            Vector3 dalgaMerkezi = ocean.transform.position;
            Vector3 gemiPozisyonu = transform.position;

            // Dalga merkezi ile gemi pozisyonu arasýndaki vektörü al
            Vector3 dalgaVektoru = gemiPozisyonu - dalgaMerkezi;

            // Dalga yüzeyine dik olan vektörü elde etmek için cross product kullan
            Vector3 dalgaYonu = Vector3.Cross(dalgaVektoru, Vector3.up).normalized;

            return dalgaYonu;
        }
        else
        {
            Debug.LogError("Deniz objesi bulunamadý!");
            return Vector3.zero;
        }
    }
    //void FixedUpdate()
    //{
    //    float horizontal = Input.GetAxis("Horizontal");
    //    float vertical = Input.GetAxis("Vertical");

    //    Vector3 hareket = new Vector3(horizontal, 0.0f, vertical);
    //    transform.Translate(hareket * hareketHizi * Time.deltaTime);

    //    float donus = horizontal * donusHizi * Time.deltaTime;
    //    transform.Rotate(Vector3.up, donus);
    //}
}
