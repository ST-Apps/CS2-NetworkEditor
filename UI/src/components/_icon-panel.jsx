import React from 'react'
import $Section from './_section';
import $Paragraph from './_paragraph';
import $Description from './_description';
import $Label from './_label';

const $IconPanel = ({ label, description, icon, fitChild, children, style }) => {
    const conditionalInsert = !fitChild ?
        <div style={{ display: 'flex', width: '75rem', justifyContent: 'center' }}>
            {children}
        </div> : ''

    const conditionalInsert2 = fitChild ?
        <div style={{ display: 'flex', width: '100%', justifyContent: 'center' }}>
            {children}
        </div> : ''

    return <$Section style={{ marginBottom: '10rem', ...style }}>
        <div style={{ display: 'flex', flexDirection: 'row' }}>
            <div style={{ display: 'flex', width: '75rem', justifyContent: 'center' }}>
                <img style={{ alignSelf: 'center', margin: '10rem', maxWidth: '55rem', maxHeight: '55rem' }} src={icon} />
            </div>
            <div style={{ flex: 1, marginTop: '10rem', marginBottom: '10rem' }}>
                <$Label isBold="true">{label}</$Label>
                <$Description style={{ padding: '0' }}>
                    <$Paragraph style={{ fontWeight: 'bold', color: 'var(--accentColorLight)' }}>
                        {description}
                    </$Paragraph>
                </$Description>
            </div>
            {conditionalInsert}
        </div>
        {conditionalInsert2}
    </$Section>
}

export default $IconPanel