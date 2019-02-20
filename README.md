# mlf-unity
The Multi Language Format interpretor for Unity3D

Note: This addon requires [Odin Inspector](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041)

### About MLF

MLF is a simple format that allows multiple human readable languages to be present in the same file. This interpretor for Unity3D imports these files, and makes them actually usable in engine.

MLF is currently utilized in-house by Spellcast Studios in Unity3D for a variety of uses, such as scripting, quests and dialog. 

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
