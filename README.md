# mlf-unity
The Multi Language Format interpretor for Unity3D

### About MLF

MLF is currently utilized in-house by Spellcast Studios in Unity3D for a variety of uses, such as scripting, quests and dialog. Since the key feature of MLF is simply to be a human readable wrapper for other languages, it allows different formats to be in the same file, with different tags and id's for the engine to read.

### Supported Languages

The Core of MLF does not support any languages by itself - instead, MLF Processors can be added to attach new formats and other features to the interpretor.

Alongside Core, this repo also has a simple YAML formatter, which automatically deserializes the content using YamlDotNet. This is mainly intended as an example of the usage of MLF.

### Examples

Here is an example of how Spellcast Studios uses MLF for Dialog:

```
@yaml
[[Dialog "Topic"]]
- topic: "Where are you from?"
  onselect:
    speak_select:
      selector: random
      values:
        - "I've lived in Eastern Aruden, the Wetlands, for all my life. Someday I wish to travel the world."
        - "I've been stuck here for all my years. I wish I wasn't"
        - "The Wetlands, I'm afraid. Been here all my life."
    options:
        - value: "Well, the wetlands has it's own beauty"
          speak: "I suppose so, friend. I suppose so."
        - value: "I too wish to explore the world, I hate staying in one place for too long"
          speak: "I have also grown sick of being here..."
```

In this case, our dialog system deserializes this YAML block during runtime, and then steps through it as it desires. 
