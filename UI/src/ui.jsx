//import React from 'react';
//import { useDataUpdate } from 'hookui-framework';
//import $TabWindow from './components/_tab-window';
//import $Settings from './tabs/_settings';
//import $ZoneColours from './tabs/_zone-colours';
//import $ZoneSettings from './tabs/_zone-settings';
//import $About from './tabs/_about';

//const $LegacyFlavour = ({ react }) => {

//    const [data, setData] = react.useState({})

//    useDataUpdate(react, "cities2modding_legacyflavour.config", setData)

//    const triggerUpdate = (prop, val) => {
//        engine.trigger("cities2modding_legacyflavour.updateProperty", JSON.stringify({ property: prop, value: val }) );
//    };

//    const toggleVisibility = () => {        
//        const data = { type: "toggle_visibility", id: "cities2modding.legacyflavour" };
//        const event = new CustomEvent('hookui', { detail: data });
//        window.dispatchEvent(event);
//    }

//    const tabs = [
//        {
//            name: 'Settings',
//            content: <div style={{ display: 'flex', width: '100%' }}>
//                <$Settings react={react} data={data} triggerUpdate={triggerUpdate} />
//            </div>
//        },
//        {
//            name: 'Zone Settings',
//            content: <div style={{ height: '100%', width: '100%' }}>
//                <$ZoneSettings react={react} data={data} triggerUpdate={triggerUpdate} />
//            </div>
//        },
//        {
//            name: 'Zone Colours',
//            content: <div style={{ height: '100%', width: '100%' }}>
//                <$ZoneColours react={react} data={data} triggerUpdate={triggerUpdate} />
//            </div>
//        },
//        {
//            name: 'About',
//            content: <div style={{ height: '100%', width: '100%' }}>
//                <$About />
//            </div>
//        }
//    ];

//    return <$TabWindow react={react} tabs={tabs} onClose={toggleVisibility} />
//};

//// Registering the panel with HookUI
//window._$hookui.registerPanel({
//    id: "cities2modding.legacyflavour",
//    name: "Legacy Flavour",
//    icon: "Media/Game/Icons/GenericVehicle.svg",
//    component: $LegacyFlavour
//});

import React from 'react';
import { useDataUpdate, $Panel, $Field } from 'hookui-framework'
import { createRoot } from 'react-dom/client';
import { createPortal } from 'react-dom';
import $Select from './components/_select';

const eventsNamespaceKey = "NetworkEditor";
const eventEdgeUpdatedKey = `${eventsNamespaceKey}.EdgeUpdated`;
const currentEdgeKey = `${eventsNamespaceKey}.CurrentEdge`;

const $Editor = ({ react }) => {
    // This sets up the currentControlPoint as local state
    const [data, setData] = react.useState({
        "Edge": {},
        "General": {},
        "Left": {},
        "Right": {}
    });

    //engine.on('TEST_EVENT', (uiEdge) => {
    //    console.log(uiEdge);
    //});

    const handleCheckboxChange = (section, flag) => (event) => {
        console.log(`Setting data[${section}][${flag}] to ${event}`)
        data[section][flag] = event;
        engine.trigger(eventEdgeUpdatedKey, data);

        setData({
            ...data,
            [section]: {
                ...data[section],
                [flag]: event
            }
        });
    }

    const renderCheckboxes = (section) => {
        return (
            <$Select react={react}
                section={section}
                options={data[section]}
                style={{ margin: '10rem', flex: '1' }}
                onToggle={(sec, flag) => handleCheckboxChange(sec, flag)}>
            </$Select>
        );
    };

    // useDataUpdate binds the result of the GetterValueBinding to currentControlPoint
    useDataUpdate(react, currentEdgeKey, setData)

    return <$Panel react={react} title="Network Editor">
        <div>Editing Edge: {data.Edge.index}</div>
        <div>
            <h3>General</h3>
            {renderCheckboxes('General')}
            <h3>Left</h3>
            {renderCheckboxes('Left')}
            <h3>Right</h3>
            {renderCheckboxes('Right')}
        </div>
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
