using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
	[SerializeField] private HexNode _hexNode;
	public HexNode _HexNode => this._hexNode;

	private void Awake()
	{
		this._hexNode.HexTile = this;
	}
}