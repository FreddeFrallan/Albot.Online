using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SortingGame {

    public class SortingAnimator : MonoBehaviour{

        [SerializeField]
        private Color normalColor, compareColor, swap1Color, swap2Color;

        private int actionIndex = -1;
        private List<SortAction> actions = new List<SortAction>();
        private SortingVisualNumber[] numbers;
        private List<int> oldEffect = new List<int>();

        private float leftPressed = 0, rightPressed = 0;
        private float firstPressStamp = 0.5f;
        private float pressTimeStamp = 0.04f;

        public void init(SortingVisualNumber[] numbers) {
            this.numbers = numbers;
            actionIndex = 0;
            actions = new List<SortAction>() { new SortAction() { type = ActionType.Empty, indexes = new List<int>()} };
            oldEffect = new List<int>();
        }

        void Update() {
            if (Input.GetKey(KeyCode.LeftArrow)) {
                if(leftPressed == 0 && hasPrev())
                    playPastAction();
                leftPressed += Time.deltaTime;

                if(leftPressed >= firstPressStamp + pressTimeStamp) {
                    leftPressed = firstPressStamp;
                    if(hasPrev())
                        playPastAction();
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
                leftPressed = 0;


            if (Input.GetKey(KeyCode.RightArrow)) {
                if (rightPressed == 0 && hasNext())
                    playNextAction();
                rightPressed += Time.deltaTime;

                if (rightPressed >= firstPressStamp + pressTimeStamp) {
                    rightPressed = firstPressStamp;
                    if(hasNext())
                        playNextAction();
                }
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
                rightPressed = 0;
        }


        #region Effects
        private void visualizeAction(int index, bool prev = false) {
            SortAction action = actions[index];

            if (action.type == ActionType.Compare) {
                resetOldEffect();
                colorEffect(action.indexes[0], action.indexes[1], compareColor, compareColor);
            }
            else if (action.type == ActionType.PreSwap && prev == false) {
                resetOldEffect();
                colorEffect(action.indexes[0], action.indexes[1], swap1Color, swap2Color);
            }
            else if (action.type == ActionType.Swap) {
                swapEffect(action.indexes[0], action.indexes[1], prev);
                actionIndex += prev ? -1 : 1;
            }
            else if (action.type == ActionType.PostSwap && prev) {
                resetOldEffect();
                colorEffect(action.indexes[0], action.indexes[1], swap2Color, swap1Color);
            }
            else if(action.type == ActionType.Empty) {
                resetOldEffect();
            }
            oldEffect.AddRange(action.indexes);
        }

        private void colorEffect(int i1, int i2, Color c1, Color c2) {
            numbers[i1].setColor(c1);
            numbers[i2].setColor(c2);
        }

        private void swapEffect(int i1, int i2, bool reverse = false) {
            float temp = numbers[i1].value;
            numbers[i1].assignNewValue(numbers[i2].value);
            numbers[i2].assignNewValue(temp);

            if (reverse)
                colorEffect(i1, i2, swap1Color, swap2Color);
            else
                colorEffect(i1, i2, swap2Color, swap1Color);
        }

        private void resetOldEffect() {
            oldEffect.ForEach(i => numbers[i].setColor(normalColor));
            oldEffect.Clear();
        }
        #endregion

        #region API
        public void playPastAction() { visualizeAction(--actionIndex, true); }
        public void playNextAction() { visualizeAction(++actionIndex); }
        public bool hasNext() { return actions.Count > actionIndex+1 && actions.Count > 0; }
        public bool hasPrev() { return actionIndex > 0 && actions.Count > 0; }
        public void addAction(SortAction action) {
            if(action.type == ActionType.Swap) {
                actions.Add(new SortAction() { type = ActionType.PreSwap, indexes = action.indexes});
                actions.Add(action);
                actions.Add(new SortAction() { type = ActionType.PostSwap, indexes = action.indexes });
            }
            else
                actions.Add(action);
        }
        #endregion
    }

    public enum ActionType {Compare, PreSwap, Swap, PostSwap,Empty}
    public struct SortAction {
        public ActionType type;
        public List<int> indexes;
    }

}