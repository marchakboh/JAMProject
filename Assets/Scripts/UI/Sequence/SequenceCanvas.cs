using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceCanvas : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    
    private Character player_character;
    private GameObject current_sequence_object;
    
    public void Init(Character character)
    {
        player_character = character;
    }

    public void NextSequenceIteration()
    {
        int index = Random.Range(0, items.Length);
        current_sequence_object = items[index];
        
        SequenceItem item = current_sequence_object.GetComponent<SequenceItem>();
        item.OnFailed += OnSequenceFail;
        int x_pos = Random.Range(-250, 250);
        int y_pos = Random.Range(-100, 60);
        item.ResetPosition(x_pos, y_pos);
        item.ActivateSequence();
    }

    public bool ActionPerform(string key)
    {
        if (!current_sequence_object) return false;

        SequenceItem item = current_sequence_object.GetComponent<SequenceItem>();
        bool result = item.IsKey(key);
        if (!result)
        {
            OnSequenceFail();
        }
        return result;
    }

    private void OnSequenceFail()
    {
        SequenceItem item = current_sequence_object.GetComponent<SequenceItem>();
        if (item)
            item.OnFailed -= OnSequenceFail;
        player_character.FailSequence();
    }
}
