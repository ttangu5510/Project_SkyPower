using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL 
{
    public class PopUpUI : MonoBehaviour
    {
        // TODO: 팝업 되면 팝업 판넬 범위 바깥을 1. 클릭하지 못하게 막거나, 2. 클릭 시 팝업 이전으로 돌아가게 한다.
        private Stack<BaseUI> stack = new Stack<BaseUI>();
        public void PushUIStack(BaseUI ui)
        {
            if (stack.Count > 0)
            {
                BaseUI top = stack.Peek();
                top.gameObject.SetActive(false);
            }
            stack.Push(ui);

            //blocker.SetActive(true);
        }
        public void PopUIStack()
        {
            if(stack.Count<=0) return;

            
            Destroy(stack.Pop().gameObject);

            if(stack.Count >0)
            {
                BaseUI top = stack.Peek();
                top.gameObject.SetActive(true);
            }
            else
            {
                //blocker.SetActive(false);
            }

        }
    }
}

