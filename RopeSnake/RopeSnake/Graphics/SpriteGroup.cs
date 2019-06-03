using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Drawing;
using RopeSnake.Helpers;
using System.Collections;

namespace RopeSnake.Graphics
{
    public class SpriteGroup : IEnumerable<Sprite>
    {
        private Sprite[] _sprites;

        public int Count => _sprites.Length;
        
        public Sprite this[int index] => _sprites[index];

        public SpriteGroup(IEnumerable<Sprite> sprites)
        {
            sprites.ThrowIfNull(nameof(sprites));

            _sprites = sprites.ToArray();
        }

        public RectangleR GetBounds()
        {
            if (Count == 0)
                return new RectangleR(0, 0, 0, 0);

            var firstSprite = _sprites[0];
            var firstBounds = firstSprite.GetBounds();

            return _sprites.Skip(1).Aggregate(firstBounds, (agg, spr) => agg.Union(spr.GetBounds()));
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return ((IEnumerable<Sprite>)_sprites).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Sprite>)_sprites).GetEnumerator();
        }
    }
}
