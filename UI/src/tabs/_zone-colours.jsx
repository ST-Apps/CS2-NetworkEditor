import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $ColorPicker from '../components/_colorpicker';
import $Button from '../components/_button';
import $Select from '../components/_select';

const $ZoneColours = ({ react, data, triggerUpdate }) => {
    const noneString = "Default Colours";
    const colourModes = [
        noneString,
        "Deuteranopia",
        "Protanopia",
        "Tritanopia",
        "Custom"
    ];
    const onModeChanged = (selected) => {
        triggerUpdate("Mode", selected == noneString ? "None" : selected);
    };

    const triggerZoneColourUpdate = (zoneName, colour) => {
        engine.trigger("cities2modding_legacyflavour.updateZoneColour", zoneName, colour);
    };

    let zoneGroups = [
        { name: "Residential", icon: "ZoneResidential", desc: "Modify Residential zone colours", items: [] },
        { name: "Commercial", icon: "ZoneCommercial", desc: "Modify Commercial zone colours", items: [] },
        { name: "Office", icon: "ZoneOffice", desc: "Modify Office zone colours", items: [] },
        { name: "Industrial", icon: "ZoneIndustrial", desc: "Modify Industrial zone colours", items: [] }
    ];

    if (data.Zones) {
        data.Zones.map((zone, index) => {
            var name = zone.Name;

            for (var i = 0; i < zoneGroups.length; i++) {
                if (name.startsWith(zoneGroups[i].name)) {
                    console.log(zoneGroups[i].name);
                    zoneGroups[i].items.push(zone);
                    break;
                }
            }
        });
    }

    const getZoneColours = (zoneGroup) => {
        let icon = "coui://legacyflavourui/Icons/" + zoneGroup.icon + "_" + data.Mode + ".svg"; // Cache busting via querystring causes flickers so meh! We need a game restart for icon changes

        return (<$IconPanel key={zoneGroup.name} label={zoneGroup.name} style={{ flex: 1 }}
            description={zoneGroup.desc}
            icon={icon} fitChild="true">
            <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
                {
                    zoneGroup.items.map((zone, index) => {
                        let colour = zone.Colour;

                        if (data.Mode == "Deuteranopia" && zone.Deuteranopia != "default")
                            colour = zone.Deuteranopia;
                        else if (data.Mode == "Protanopia" && zone.Protanopia != "default")
                            colour = zone.Protanopia;
                        else if (data.Mode == "Tritanopia" && zone.Tritanopia != "default")
                            colour = zone.Tritanopia;
                        else if (data.Mode == "Custom" && zone.Custom != "default")
                            colour = zone.Custom;

                        const onChanged = (newColour) => {
                            triggerZoneColourUpdate(zone.Name, newColour);
                        };

                        return (<$ColorPicker key={zone.Name} react={react} label={zone.Name} color={colour} onChanged={onChanged} />);
                    })
                }
            </div>
        </$IconPanel>)
    };

    const renderZoneColours = (index) => {
        return getZoneColours(zoneGroups[index]);
    };

    const triggerRegenerateIcons = () => {
        engine.trigger("cities2modding_legacyflavour.regenerateIcons");
    };

    const triggerSetColoursToVanilla = () => {
        engine.trigger("cities2modding_legacyflavour.setColoursToVanilla");
    };

    const triggerResetColoursToDefault = () => {
        engine.trigger("cities2modding_legacyflavour.resetColoursToDefault");
    };

    const modeString = data.Mode == "None" ? noneString : data.Mode;

    return <div>
        <div style={{ display: 'flex', flexDirection: 'row' }}>
            <div style={{ width: '66.666666666%', paddingRight: '5rem' }}>
                <$IconPanel label="Colour Blindness Mode"
                    description="Select a different colour mode, you can cycle these with the keys SHIFT+Z."
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <$Select react={react} selected={modeString} options={colourModes} style={{ margin: '10rem', flex: '1' }} onSelectionChanged={onModeChanged}></$Select>
                </$IconPanel>
            </div>
            <div style={{ width: '33.33333333333%' }}>
                <$Button onClick={triggerRegenerateIcons}>Regenerate icons (Game restart required)</$Button>
                <$Button style={{ marginTop: '5rem' }} onClick={triggerSetColoursToVanilla}>Set {modeString}&nbsp;to vanilla colours</$Button>
                <$Button style={{ marginTop: '5rem' }} onClick={triggerResetColoursToDefault}>Reset {modeString}</$Button>
            </div>
        </div>
        <div style={{ display: 'flex', width: '100%', flexDirection: 'row' }}>
            <div style={{ flex: 1, width: '33.33333333333%' }}>
                {renderZoneColours(0)}
            </div>
            <div style={{ flex: 1, width: '33.33333333333%', paddingLeft: '5rem', paddingRight: '5rem' }}>
                {renderZoneColours(1)}
                {renderZoneColours(2)}
            </div>
            <div style={{ flex: 1, width: '33.33333333333%', paddingLeft: '5rem' }}>
                {renderZoneColours(3)}
            </div>
        </div>
    </div>
}

export default $ZoneColours