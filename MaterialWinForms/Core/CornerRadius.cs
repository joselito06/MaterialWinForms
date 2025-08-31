using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Configuración de esquinas redondeadas
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CornerRadius
    {
        private int _all = 8;
        private int _topLeft = 8;
        private int _topRight = 8;
        private int _bottomLeft = 8;
        private int _bottomRight = 8;
        private bool _useIndividual = false;

        [DefaultValue(8)]
        public int All
        {
            get => _all;
            set
            {
                _all = Math.Max(0, value);
                if (!_useIndividual)
                {
                    _topLeft = _topRight = _bottomLeft = _bottomRight = _all;
                }
            }
        }

        [DefaultValue(false)]
        public bool UseIndividualCorners
        {
            get => _useIndividual;
            set => _useIndividual = value;
        }

        [DefaultValue(8)]
        public int TopLeft
        {
            get => _topLeft;
            set => _topLeft = Math.Max(0, value);
        }

        [DefaultValue(8)]
        public int TopRight
        {
            get => _topRight;
            set => _topRight = Math.Max(0, value);
        }

        [DefaultValue(8)]
        public int BottomLeft
        {
            get => _bottomLeft;
            set => _bottomLeft = Math.Max(0, value);
        }

        [DefaultValue(8)]
        public int BottomRight
        {
            get => _bottomRight;
            set => _bottomRight = Math.Max(0, value);
        }

        public CornerRadius() { }

        public CornerRadius(int all)
        {
            All = all;
        }

        public CornerRadius(int topLeft, int topRight, int bottomLeft, int bottomRight)
        {
            _useIndividual = true;
            _topLeft = Math.Max(0, topLeft);
            _topRight = Math.Max(0, topRight);
            _bottomLeft = Math.Max(0, bottomLeft);
            _bottomRight = Math.Max(0, bottomRight);
        }

        public override string ToString()
        {
            return _useIndividual
                ? $"TL={TopLeft}, TR={TopRight}, BL={BottomLeft}, BR={BottomRight}"
                : $"All={All}";
        }
    }
}
