import React,{ useState } from 'react'
import { AppBar,Tab,Tabs,Toolbar,Typography,Button,Drawer,useMediaQuery,useTheme } from '@mui/material';
import ShoppingCartCheckoutIcon from '@mui/icons-material/ShoppingCartCheckout';
import { Link,Route,Switch } from 'react-router-dom'; // Import Link and Route
import DrawerComp from './DrawerComp';

const PAGES = ["Home","Admin","Vendor","About Us","Contact Us"];

const Header = () => {

    const [value,setValue] = useState();
    const theme = useTheme();
    console.log(theme);
    const isMatch = useMediaQuery(theme.breakpoints.down('md'));
    console.log(isMatch);

    return (
        <React.Fragment >
            <AppBar sx={{
                background: '#063970'
            }}>
                < Toolbar >
                    <Typography>
                        🅴🆂🅷🅾🅿
                    </Typography>
                    <ShoppingCartCheckoutIcon />
                    {
                        isMatch ? (
                            <>
                                <Typography sx={{ fontSize: '1.5rem',paddingLeft: '10%' }}>
                                    🅴🆂🅷🅾🅿
                                </Typography>
                                <DrawerComp />
                            </>
                        ) : (
                            <>
                                <Tabs sx={{ marginLeft: "auto" }} textColor="inherit" value={value} onChange={(e,value) => setValue(value)}>
                                    {
                                        PAGES.map((page,index) => (
                                            <Tab key={index} label={page} component={Link} to={`/${page.replace(" ","-").toLowerCase()}`} />
                                        ))
                                    }
                                    {/*<Tab label="Products" />
                                    < Tab label="Services" />
                                    <Tab label="Contact Us" />
                                    <Tab label="About Us" />*/}
                                </Tabs>
                                <Button sx={{ marginLeft: 'auto' }} variants="contained" > Login</Button>
                            </>
                        )
                    }

                </Toolbar>

            </AppBar >
        </React.Fragment >
    )
}

export default Header