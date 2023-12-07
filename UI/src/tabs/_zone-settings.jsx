import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $CheckBox from '../components/_checkbox';
import $Slider from '../components/_slider';
import $Button from '../components/_button';

const $ZoneSettings = ({ react, data, triggerUpdate }) => {
    const triggerResetZoneSettingsToDefault = () => {
        engine.trigger("cities2modding_legacyflavour.resetZoneSettingsToDefault");
    };
    return <div style={{ width: '100%', display: 'flex', flexDirection: 'row' }}>
        <div style={{ flex: 1, width: '50%' }}>
            <div style={{ flex: 1, paddingRight: '5rem' }}>
                <$IconPanel label="Custom Zone Colouring"
                    description="Provides custom zone colour options that can be cycled with a key shortcut. Toggle with ALT+Z."
                    icon="Media/Game/Icons/Zones.svg">
                    <$CheckBox style={{ alignSelf: 'center', margin: '10rem' }} checked={data.Enabled} onToggle={(val) => triggerUpdate("Enabled", val)} />
                </$IconPanel>
                <$IconPanel label="Use Dynamic Cell Borders"
                    description="Zone cell borders will adjust to be more visible when there is snow coverage."
                    icon="Media/Game/Climate/Snow.svg">
                    <$CheckBox style={{ alignSelf: 'center', margin: '10rem' }} checked={data.UseDynamicCellBorders} onToggle={(val) => triggerUpdate("UseDynamicCellBorders", val)} />
                </$IconPanel>
                <$IconPanel label="Cell Opacity"
                    description="Change the transparency of non-empty zone cells."
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <$Slider react={react} value={data.CellOpacity} onValueChanged={(val) => triggerUpdate("CellOpacity", val)} />
                </$IconPanel>
                <$IconPanel label="Cell Border Opacity"
                    description="Change the border transparency of non-empty zone cells."
                    icon="Media/Editor/Edit.svg" fitChild="true">
                    <$Slider react={react} value={data.CellBorderOpacity} onValueChanged={(val) => triggerUpdate("CellBorderOpacity", val)} />
                </$IconPanel>
            </div>
        </div>
        <div style={{ flex: 1, width: '50%', paddingLeft: '5rem' }}> 
            <$IconPanel label="Empty Cell Opacity"
                description="Change the transparency of empty zone cells."
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Slider react={react} value={data.EmptyCellOpacity} onValueChanged={(val) => triggerUpdate("EmptyCellOpacity", val)} />
            </$IconPanel>
            <$IconPanel label="Empty Cell Border Opacity"
                description="Change the border transparency of empty zone cells."
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Slider react={react} value={data.EmptyCellBorderOpacity} onValueChanged={(val) => triggerUpdate("EmptyCellBorderOpacity", val)} />
            </$IconPanel>
            <$Button style={{ marginTop: '5rem' }} onClick={triggerResetZoneSettingsToDefault}>Reset to default</$Button>
        </div>
    </div>
}

export default $ZoneSettings