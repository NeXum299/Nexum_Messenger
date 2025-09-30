import { useState } from 'react';
import Search from './Search/Search.jsx';
import styles from './ListHeader.module.css';
import ButtonsFilter from './ButtonsFilter/ButtonsFilter.jsx';
import ButtonMenu from './ButtonMenu/ButtonMenu.jsx';
import Sidebar from '../Sidebar/Sidebar.jsx';
import MyProfile from '../Sidebar/MyProfile/MyProfile.jsx';
import Customization from '../Sidebar/Customization/Customization.jsx';
import CreateGroup from '../Sidebar/CreateGroup/CreateGroup.jsx';
import CreateChannel from '../Sidebar/CreateChannel/CreateChannel.jsx';
import Policy from '../Sidebar/Policy/Policy.jsx';
import AddFriend from '../Sidebar/AddFriend/AddFriend.jsx';
import FriendRequests from '../Sidebar/FriendRequests/FriendRequests.jsx';

export default function ListHeader() {
    const [isSidebarOpen, setIsSidebarOpen] = useState(false);
    const [isClosing, setIsClosing] = useState(false);

    const [myProfileOpen, setMyProfileOpen] = useState(false);
    const [customizationOpen, setCustomizationOpen] = useState(false);
    const [createGroupOpen, setCreateGroupOpen] = useState(false);
    const [createChannelOpen, setCreateChannelOpen] = useState(false);
    const [policyOpen, setPolicyOpen] = useState(false);
    const [addFriendOpen, setAddFriendOpen] = useState(false);
    const [friendRequestsOpen, setFriendRequestsOpen] = useState(false);

    const handleMenuClick = () => {
        setIsSidebarOpen(true);
        setIsClosing(false);
    };

    const handleCloseSidebar = () => {
        setIsClosing(true);
        setTimeout(() => {
            setIsSidebarOpen(false);
            setIsClosing(false);
        }, 300);
    };

    return (
        <>
            <div className={styles["header-content"]}>
                <div className={styles["search-container"]}>
                    <ButtonMenu onClick={handleMenuClick} />
                    <Search />
                </div>
                <div className={styles["buttons-container"]}>
                    <ButtonsFilter />
                </div>
            </div>

            {isSidebarOpen && (
                <div 
                    className={`${styles["sidebar-overlay"]} ${isClosing ? styles["closing"] : ""}`}
                    onClick={handleCloseSidebar}>
                    <div 
                        className={styles["sidebar-container"]}
                        onClick={(e) => e.stopPropagation()}>
                        <Sidebar 
                            onMyProfileOpen={() => setMyProfileOpen(true)}
                            onCustomizationOpen={() => setCustomizationOpen(true)}
                            onCreateGroupOpen={() => setCreateGroupOpen(true)}
                            onCreateChannelOpen={() => setCreateChannelOpen(true)}
                            onPolicyOpen={() => setPolicyOpen(true)}
                            onAddFriendOpen={() => setAddFriendOpen(true)}
                            onFriendRequestsOpen={() => setFriendRequestsOpen(true)}
                        />
                    </div>
                </div>
            )}

            {myProfileOpen && <MyProfile onClose={() => setMyProfileOpen(false)} />}
            {customizationOpen && <Customization onClose={() => setCustomizationOpen(false)} />}
            {createGroupOpen && <CreateGroup onClose={() => setCreateGroupOpen(false)} />}
            {createChannelOpen && <CreateChannel onClose={() => setCreateChannelOpen(false)} />}
            {policyOpen && <Policy onClose={() => setPolicyOpen(false)} />}
            {addFriendOpen && <AddFriend onClose={() => setAddFriendOpen(false)} />}
            {friendRequestsOpen && <FriendRequests onClose={() => setFriendRequestsOpen(false)} />}
        </>
    );
}
