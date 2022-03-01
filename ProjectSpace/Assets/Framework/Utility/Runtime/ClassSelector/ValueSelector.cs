#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace Framework.Utility.Runtime
{
    public class ValueSelector<T> : OdinSelector<ValueDropdownItem<T>>
    {
        /// <summary>
        /// 内容
        /// </summary>
        private List<ValueDropdownItem<T>> m_Values;

        private readonly bool m_NeedThumbnailIcons;
        private readonly bool m_ConfirmSelectionOnDoubleClick;

        public ValueSelector()
        {
        }

        public ValueSelector(List<ValueDropdownItem<T>> values, Action<T> confirmed, bool needThumbnailIcons = false, bool confirmSelectionOnDoubleClick = true)
        {
            m_Values = values;
            m_NeedThumbnailIcons = needThumbnailIcons;
            m_ConfirmSelectionOnDoubleClick = confirmSelectionOnDoubleClick;
            this.SelectionConfirmed += (enumerable =>
            {
                var firstOrDefault = enumerable.FirstOrDefault();
                confirmed?.Invoke(firstOrDefault.Value);
            });
        }

        protected override void BuildSelectionTree(OdinMenuTree tree)
        {
            tree.Selection.SupportsMultiSelect = true;
            tree.Config.DrawSearchToolbar = true;
            tree.Config.ConfirmSelectionOnDoubleClick = m_ConfirmSelectionOnDoubleClick;
            var odinMenuItems = tree.AddRange(m_Values, (arg => arg.Text));
            if (m_NeedThumbnailIcons)
            {
                odinMenuItems.AddThumbnailIcons();
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        public void Show()
        {
            if (SelectionTree.EnumerateTree().Count() == 1)
            {
                // If there is only one scriptable object to choose from in the selector, then 
                // we'll automatically select it and confirm the selection. 
                SelectionTree.EnumerateTree().First().Select();
                SelectionTree.Selection.ConfirmSelection();
            }
            else
            {
                // Else, we'll open up the selector in a popup and let the user choose.
                ShowInPopup(200);
            }
        }
    }
}
#endif