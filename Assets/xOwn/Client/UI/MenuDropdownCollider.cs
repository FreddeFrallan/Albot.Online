using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ClientUI{
	public class MenuDropdownCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler{

		public MenuDropdown theMenu;


		public void OnPointerExit (PointerEventData eventData){theMenu.OnMouseExit ();}
		public void OnPointerEnter(PointerEventData eventData){theMenu.OnMouseOver ();}
	}
}