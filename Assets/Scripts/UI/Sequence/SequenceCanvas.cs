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
        item.ActivateSequence();
    }

    public bool ActionPerform(string key)
    {
        if (!current_sequence_object) return false;

        SequenceItem item = current_sequence_object.GetComponent<SequenceItem>();
        return item.IsKey(key);
    }

    private void OnSequenceFail()
    {
        player_character.FailSequence();
    }
}
