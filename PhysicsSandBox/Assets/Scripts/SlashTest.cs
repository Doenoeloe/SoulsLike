using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashTest : MonoBehaviour
{
   public List<Slash> slashes;
   
   public bool attacking;

   private void Start()
   {
      DisableSlashes();
   }

   private void Update()
   {
      if (Input.GetMouseButtonDown(0) && !attacking)
      {
         attacking = true;
         StartCoroutine(SlashAttack());
      }
   }

   IEnumerator SlashAttack()
   {
      for (int i = 0; i < slashes.Count; i++)
      {
         yield return new WaitForSeconds(slashes[i].delay);
         slashes[i].slashObj.SetActive(true);
      }
      yield return new WaitForSeconds(1);
      DisableSlashes();
      attacking = false;
   }

   void DisableSlashes()
   {
      for (int i = 0; i < slashes.Count; i++)
         slashes[i].slashObj.SetActive(false);
   }  
}

[System.Serializable]
public class Slash
{
   public GameObject slashObj;
   public float delay;
}
