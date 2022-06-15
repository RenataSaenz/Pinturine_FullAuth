using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Draw : MonoBehaviour
{
   Camera _mainCam;
   public Brush brush;
   [SerializeField]
   Transform _brushSpawner;

   private LineRenderer currentLineRenderer;

   private Vector2 lastPos;

   [SerializeField] private List<Brush> _drawMade = new List<Brush>();
   void Start()
   {
      _mainCam = Camera.main;
   }

   public Draw SetParent(Transform parent)
   {
      transform.SetParent(parent);
      return this;
   }

   public void DrawAction()
   {
      if (Input.GetKeyDown(KeyCode.Mouse0))
      {
         CreateBrush();
      }

      if (Input.GetKey(KeyCode.Mouse0))
      {
         Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
         if (mousePos != lastPos)
         {
            AddAPoint(mousePos);
            lastPos = mousePos;
         }
      }
      else
      {
         currentLineRenderer = null;
      }
   }

   void CreateBrush()
   {
      Brush brushInstance = Instantiate(brush);
      _drawMade.Add(brushInstance);
      currentLineRenderer = brushInstance.GetComponent<LineRenderer>();

      Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
      
      currentLineRenderer.SetPosition(0, mousePos);
      currentLineRenderer.SetPosition(1, mousePos);
   }

   void AddAPoint(Vector2 pointPos)
   {
      currentLineRenderer.positionCount++;
      int positionIndex = currentLineRenderer.positionCount - 1;
      currentLineRenderer.SetPosition(positionIndex, pointPos);
   }

   public void ClearDrawing()
   {
      foreach (var d in _drawMade)
      {
         Destroy(d.gameObject);
      }
      _drawMade.Clear();
   }
}
