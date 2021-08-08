using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

#if SHAPES_URP || SHAPES_HDRP
using Shapes;
#endif

namespace PixLi
{
#if SHAPES_URP || SHAPES_HDRP
	public class PathfinderVisualizer : ImmediateModeShapeDrawer
	{
		private Data[] _data;

		public void Visualize(Data[] data)
		{
			this._data = data;
		}

		[SerializeField] private Color _distanceColor;
		[SerializeField] private Color _heuristicColor;
		[SerializeField] private Color _finalColor;

		[SerializeField] private int _zOffsetUnits;
		public int _ZOffsetUnits => this._zOffsetUnits;

		[SerializeField] private CompareFunction _zTest;
		public CompareFunction _ZTest => this._zTest;

		[SerializeField] private TextAlign _textAlign;
		public TextAlign _TextAlign => this._textAlign;

		[SerializeField] private Vector3 _textOffset;
		public Vector3 _TextOffset => this._textOffset;

		[SerializeField] private float _fontSize = 50;
		public float _FontSize => this._fontSize;

		[SerializeField] private Color _textBackgroundColor = Color.blue;
		public Color _TextBackgroundColor => this._textBackgroundColor;

		[SerializeField] private float _textBackgroundCornerRadius = 0.4f;
		public float _TextBackgroundCornerRadius => this._textBackgroundCornerRadius;

		[SerializeField] private Vector2 _textBackgroundSize = new Vector2(x: 1.0f, y: 1.0f);
		public Vector2 _TextBackgroundSize => this._textBackgroundSize;

		[SerializeField] private Vector2 _textBackgroundOffset = new Vector2(x: 0.0f, y: 0.0f);
		public Vector2 _TextBackgroundOffset => this._textBackgroundOffset;

		private static Quaternion UP = Quaternion.Euler(90.0f, 0.0f, 0.0f);

		public override void DrawShapes(Camera cam)
		{
			using (Draw.Command(cam))
			{
				Draw.ZOffsetUnits = this._zOffsetUnits;
				Draw.ZTest = this._zTest;
				Draw.ThicknessSpace = ThicknessSpace.Meters;
				Draw.TextAlign = this._textAlign;

				if (this._data != null)
				{
					for (int a = 0; a < this._data.Length; a++)
					{
						Vector3 pos = this._data[a].Position - this._textOffset;

						Vector2 size = new Vector2(x: this._textBackgroundSize.x, y: this._textBackgroundSize.y) + this._textBackgroundOffset;

						Draw.Rectangle(
							pos: pos,
							rot: UP,
							rect: new Rect(
								position: new Vector2(x: this._textBackgroundOffset.x / 2.0f - size.x / 2.0f, y: this._textBackgroundOffset.y / 2.0f - size.y / 2.0f),
								size: size
							),
							cornerRadius: this._textBackgroundCornerRadius,
							color: this._textBackgroundColor
						);

						Draw.Text(
							pos: pos + new Vector3(-0.7f, 0.0f, 0.5f),
							rot: UP,
							content: $"D:{this._data[a].Distance}",
							fontSize: this._fontSize
						);

						Draw.Text(
							pos: pos + new Vector3(0.7f, 0.0f, 0.5f),
							rot: UP,
							content: $"H:{this._data[a].Heuristic}",
							fontSize: this._fontSize
						);

						Draw.Text(
							pos: pos + new Vector3(0.0f, -0.0f, -0.5f),
							rot: UP,
							content: $"F:{this._data[a].Distance + this._data[a].Heuristic}",
							fontSize: this._fontSize
						);
					}
				}
			}
		}

		public struct Data
		{
			public Data(Vector3 position, Quaternion rotation, float distance, float heuristic) : this()
			{
				this.Position = position;
				this.Rotation = rotation;
				this.Distance = distance;
				this.Heuristic = heuristic;
			}

			public Vector3 Position { get; set; }
			public Quaternion Rotation { get; set; }

			public float Distance { get; set; }
			public float Heuristic { get; set; }
		}
	}
#endif
}