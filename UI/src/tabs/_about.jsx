import React from 'react'
import $IconPanel from '../components/_icon-panel';
import $Button from '../components/_button';
import $Paragraph from '../components/_paragraph';
import $Description from '../components/_description';

const $About = ({}) => {
    const launchReddit = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://www.reddit.com/r/cities2modding");
    };
    const launchGitHub = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://github.com/Cities2Modding");
    };
    const launchDiscord = (url) => {
        engine.trigger("cities2modding_legacyflavour.launchUrl", "https://discord.gg/KGRNBbm5Fh");
    };
    return <div>
        <$IconPanel label="Legacy Flavour v0.0.4" style={{ flex: 1 }}
            description="This mod was developed by the optimus-code and the Cities2Modding community."
            icon="Media/Editor/Object.svg" fitChild="true">
        </$IconPanel>
        <div style={{ display: 'flex', flexDirection: 'column', width: '100%' }}>
            <$Description style={{ paddingTop: '0' }}>
                <$Paragraph>
                    Our mods, including "Legacy Flavour," are part of a vibrant creative effort, officially distributed via <b>Thunderstore.io</b>&nbsp;and our GitHub repository. They represent not just our work but the spirit of collaboration and innovation within the Cities Skylines modding community.
                </$Paragraph>
                <$Paragraph>
                    Special thanks to <b>Captain_Of_Coit</b>, <b>89pleasure</b>, and <b>Rebecca</b>, as well as the extensive <b>Cities2Modding</b>&nbsp;community.
                </$Paragraph>
                <$Paragraph>
                    If modding interests you, whether you're a seasoned creator or a newcomer, our community doors are always open. We thrive on shared knowledge and fresh ideas, and we believe that everyone has something unique to contribute to our growing and dynamic community.
                </$Paragraph>
            </$Description>
        </div>
        <div style={{ display: 'flex', flexDirection: 'row', flex: 1 }}>
            <div style={{ flex: 1 }}>
                <$IconPanel label="GitHub"
                    description="You can download our mods on GitHub, or even get involved if you desire!"
                    icon="https://raw.githubusercontent.com/prplx/svg-logos/master/svg/github-icon.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchGitHub}>https://github.com/Cities2Modding</$Button>
                    </div>
                </$IconPanel>
                <$IconPanel label="Reddit"
                    description="We have a sub-reddit if you would like to join or visit."
                    icon="https://www.svgrepo.com/download/14413/reddit.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchReddit}>https://www.reddit.com/r/cities2modding</$Button>
                    </div>
                </$IconPanel>
            </div>
            <div style={{ flex: 1, marginLeft: '5rem' }}>
                <$IconPanel label="Discord"
                    description="If you would like to get involved in our community or need help or support, you can find us on Discord."
                    icon="https://assets-global.website-files.com/6257adef93867e50d84d30e2/653714c1f22aef3b6921d63d_636e0a6ca814282eca7172c6_icon_clyde_white_RGB.svg" fitChild="true">
                    <div style={{ display: 'flex', flexDirection: 'column', width: '100%', padding: '10rem' }}>
                        <$Button isBlack="true" onClick={launchDiscord}>https://discord.gg/KGRNBbm5Fh</$Button>
                    </div>
                </$IconPanel>
            </div>
        </div>
    </div>
}

export default $About