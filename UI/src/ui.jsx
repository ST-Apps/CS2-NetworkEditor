import React from 'react';
import { useDataUpdate, $Panel, $Field } from 'hookui-framework'
import { createRoot } from 'react-dom/client';
import { createPortal } from 'react-dom';
import $Select from './components/_select';
import $IconPanel from './components/_icon-panel';
import $Button from './components/_button';
import $Slider from './components/_slider';

const eventsNamespaceKey = "NetworkEditor";
const eventEdgeUpdatedKey = `${eventsNamespaceKey}.EdgeUpdated`;
const currentEdgeKey = `${eventsNamespaceKey}.CurrentEdge`;
const applyChangesKey = `${eventsNamespaceKey}.ApplyChanges`;
const isConfigurationValidKey = `${eventsNamespaceKey}.ConfigurationValid`;

const $Editor = ({ react }) => {
    const [data, setData] = react.useState({
        "edge": {
            "entity": {},
            "flags": {
                "general": {},
                "left": {},
                "right": {}
            },
        },
        "startNode": {
            "entity": {},
            "flags": {
                "general": {},
                "left": {},
                "right": {}
            }
        },
        "endNode": {
            "entity": {},
            "flags": {
                "general": {},
                "left": {},
                "right": {}
            }
        }
    });
    useDataUpdate(react, currentEdgeKey, setData)

    const [configurationValid, setConfigurationValid] = react.useState(true);
    useDataUpdate(react, isConfigurationValidKey, setConfigurationValid);

    const handleCheckboxChange = (type, section, flag) => (event) => {
        console.log(`Setting data[${type}].flags[${section}][${flag}] to ${event}`)
        data[type].flags[section][flag] = event;
        engine.trigger(eventEdgeUpdatedKey, data);

        setData(prevData => ({
            ...prevData,
            [type]: {
                ...prevData[type],
                flags: {
                    ...prevData[type].flags,
                    [section]: {
                        ...prevData[type].flags[section],
                        [flag]: event
                    }
                }
            }
        }));
    };

    const renderCheckboxes = (type, section) => {
        const editingText = `Editing ${type}->${section}`;
        const editingDescription = `Toggle any flag to update your ${type}.`;

        return (
            <$IconPanel label={editingText}
                description={editingDescription}
                icon="Media/Editor/Edit.svg" fitChild="true">
                <$Select react={react}
                    options={data[type].flags[section]}
                    style={{ margin: '10rem', flex: '1' }}
                    onToggle={(flag) => handleCheckboxChange(type, section, flag)}>
                </$Select>
            </$IconPanel>
        );
    };

    const applyChanges = () => {
        engine.trigger(applyChangesKey);
    }

    const handleSliderChanged = (propertyName, value) => {
        console.log(`Setting data['edge'][${propertyName}] to ${value}`)
        data['edge'][propertyName] = value;

        engine.trigger(eventEdgeUpdatedKey, data);
    }

    return <$Panel react={react} title="Network Editor">
        <div>
            {renderCheckboxes('edge', 'general')}
            {renderCheckboxes('edge', 'left')}
            {renderCheckboxes('edge', 'right')}
            <$Slider react={react} value={data['edge'].width} onValueChanged={(val) => handleSliderChanged('width', val)} />
            <$Slider react={react} value={data['edge'].middleOffset} onValueChanged={(val) => handleSliderChanged('middleOffset', val)} />
            <$Slider react={react} value={data['edge'].widthOffset} onValueChanged={(val) => handleSliderChanged('widthOffset', val)} />
            <$Slider react={react} value={data['edge'].nodeOffset} onValueChanged={(val) => handleSliderChanged('nodeOffset', val)} />
        </div>
        <div>
            <$Button style={{ marginTop: '5rem' }}>Reset</$Button>
            <$Button style={{ marginTop: '5rem' }} onClick={applyChanges}>Apply</$Button>
        </div>
        {/*<div>*/}
        {/*    {renderCheckboxes('startNode', 'general')}*/}
        {/*    {renderCheckboxes('startNode', 'left')}*/}
        {/*    {renderCheckboxes('startNode', 'right')}*/}
        {/*</div>*/}
        {/*<div>*/}
        {/*    {renderCheckboxes('endNode', 'general')}*/}
        {/*    {renderCheckboxes('endNode', 'left')}*/}
        {/*    {renderCheckboxes('endNode', 'right')}*/}
        {/*</div>*/}
    </$Panel>
}

// Injection Script
const injectionPoint = document.getElementsByClassName('game-main-screen_TRK')[0];

// Create a new div element
const newDiv = document.createElement('div');
newDiv.className = 'info-menu-layout_E8i';
newDiv.id = 'network-editor-root';
injectionPoint.appendChild(newDiv);

// Create a root for the new div
const root = createRoot(newDiv);

// Use a portal to render the RedSquare inside the new div
root.render(createPortal(<$Editor react={React} />, newDiv));
